using StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace StaticDataEditor
{
    public static class SummarySerializeGenerator 
    {

        private static string TabString(int num)
        {
            return StaticData.Generator.StaticDataUtility.TabString(num);
        }
        private static string GetClassName(Type editorTypeDefinationClass, bool nameSpace = true)
        {
            return StaticData.Generator.StaticDataUtility.GetGenerateClassName(editorTypeDefinationClass, StaticData.Generator.Constant.Suffix, nameSpace);
        }
        private static string GetClassLabel(Type type)
        {
            return StaticData.Generator.StaticDataUtility.GetStaticDataTypeLabel(type);
        }

        public static void GenerateCSBinaryWriteMethod(string typeName, Type type, List<Type> structTypes, bool empty,  StreamWriter writer, int tabNum)
        {
            if (writer != null)
            {
                string tab = TabString(tabNum);
                string tab1 = TabString(tabNum + 1);
                string tab2 = TabString(tabNum + 2);
                
                writer.WriteLine(tab + "private static void Write(BinaryWriter writer, {0} obj)", typeName);
                writer.WriteLine(tab + "{");
                if (!empty)
                {
                    if (type.IsClass)
                    {
                        writer.WriteLine(tab1 + "if(obj == null)");
                        writer.WriteLine(tab1 + "{");
                        writer.WriteLine(tab2 + "writer.Write(false);");
                        writer.WriteLine(tab2 + "return;");
                        writer.WriteLine(tab1 + "}");
                        writer.WriteLine(tab1 + "else");
                        writer.WriteLine(tab1 + "{");
                        writer.WriteLine(tab2 + "writer.Write(true);");
                        writer.WriteLine(tab1 + "}");
                    }
                    StaticData.Generator.StaticDataUtility.TranverseFields(type, (field) => 
                    {
                        GenerateCSBinaryWriteMethod(field.FieldType, "obj." + field.Name, structTypes, writer, tabNum + 1);
                    });
                }
                writer.WriteLine(tab + "}");
            }
        }
        
        private static void GenerateCSBinaryWriteMethod(Type type, string name, List<Type> structTypes, StreamWriter writer, int tabNum)
        {
            string tab = TabString(tabNum);
            string tab1 = TabString(tabNum + 1);
            if (type.IsEnum)
            {
                writer.WriteLine(tab + "writer.Write((int){0});", name);
            }
            else if (type.IsPrimitive || type == typeof(decimal))
            {
                writer.WriteLine(tab + "writer.Write({0});", name);
            }
            else if (type.IsValueType)
            {
                if(!structTypes.Contains(type))
                {
                    structTypes.Add(type);
                }
                
                writer.WriteLine(tab + "Write(writer, {0});", name);
            }
            else if (type == typeof(string))
            {
                writer.WriteLine(tab + "if({0} == null)", name);
                writer.WriteLine(tab + "{");
                writer.WriteLine(tab1 + "writer.Write({0});", "\"\"");
                writer.WriteLine(tab + "}");
                writer.WriteLine(tab + "else");
                writer.WriteLine(tab + "{");
                writer.WriteLine(tab1 + "writer.Write({0});", name);
                writer.WriteLine(tab + "}");
            }
            else if (type.IsSubclassOf(typeof(IStaticData)))
            {
                writer.WriteLine(tab + "if({0} == null)", name);
                writer.WriteLine(tab + "{");
                writer.WriteLine(tab1 + "writer.Write(\"{0}\");", GetClassName(type, false));
                writer.WriteLine(tab1 + "writer.Write(false);");
                writer.WriteLine(tab + "}");
                writer.WriteLine(tab + "else");
                writer.WriteLine(tab + "{");
                writer.WriteLine(tab1 + "writer.Write(StaticDataUtility.GetGenerateClassName({0}.GetType(), Constant.Suffix, false));", name);
                writer.WriteLine(tab1 + "Write(writer, {0} as IStaticData);", name);
                writer.WriteLine(tab + "}");
            }
            else if (type.IsArray)
            {
                writer.WriteLine(tab + "if({0} == null)", name);
                writer.WriteLine(tab + "{");
                writer.WriteLine(tab1 + "writer.Write(-1);");
                writer.WriteLine(tab + "}");
                writer.WriteLine(tab + "else");
                writer.WriteLine(tab + "{");

                writer.WriteLine(tab1 + "writer.Write({0}.Length);", name);
                writer.WriteLine(tab1 + "for(int {1}i = 0; {1}i < {0}.Length; {1}i ++)", name, name.Replace("[","").Replace("]", "").Replace(".", ""));
                writer.WriteLine(tab1 + "{");
                GenerateCSBinaryWriteMethod(type.GetElementType(), name + "[" + name.Replace("[", "").Replace("]", "").Replace(".", "") + "i" + "]", structTypes, writer, tabNum + 2);
                writer.WriteLine(tab1 + "}");
                writer.WriteLine(tab + "}");
            }
        }
        public static void GenerateCSBinaryReadMethod(string typeName, Type type, List<Type> structTypes, bool empty, StreamWriter writer, int tabNum)
        {
            if (writer != null)
            {
                string tab = TabString(tabNum);
                string tab1 = TabString(tabNum + 1);
                string tab2 = TabString(tabNum + 2);
                writer.WriteLine(tab + "private static void Read(BinaryFileReader reader ,out {0} obj)", typeName);
                writer.WriteLine(tab + "{");
                if (type.IsClass)
                {
                    writer.WriteLine(tab1 + "obj = null;");
                }
                else
                {
                    writer.WriteLine(tab1 + "obj = new {0}();", typeName);
                }
                if (!empty)
                {
                    if (type.IsClass)
                    {
                        writer.WriteLine(tab1 + "if(!reader.ReadBoolean())");
                        writer.WriteLine(tab1 + "{");
                        writer.WriteLine(tab2 + "return;");
                        writer.WriteLine(tab1 + "}");
                        writer.WriteLine(tab1 + "if(obj == null)", typeName);
                        writer.WriteLine(tab1 + "{");
                        writer.WriteLine(tab2 + "obj = new {0}();", typeName);
                        writer.WriteLine(tab1 + "}");
                    }
                    StaticData.Generator.StaticDataUtility.TranverseFields(type, (field) => 
                    {
                        GenerateCSBinaryReadMethod(field.FieldType, "obj." + field.Name, IsEditor(type, typeName), structTypes, writer, tabNum + 1);
                    });
                }
                writer.WriteLine(tab + "}");
            }
        }
        private static bool IsEditor(Type type, string typeName)
        {
            return typeName == type.FullName;
        }
        private static void GenerateCSBinaryReadMethod(Type type, string name, bool isEditorType,List<Type> structTypes,  StreamWriter writer, int tabNum)
        {
            string tab = TabString(tabNum);
            string tab1 = TabString(tabNum + 1);

            if (type.IsEnum)
            {
                writer.WriteLine(tab + "{0} = ({1})(reader.ReadInt32());", name, type.FullName);
            }
            else if (type.IsPrimitive || type == typeof(decimal))
            {
                writer.WriteLine(tab + "{0} = reader.Read{1}();", name, type.Name);
            }
            else if (type.IsValueType)
            {
                if(!structTypes.Contains(type))
                {
                    structTypes.Add(type);
                }
                writer.WriteLine(tab + "Read(reader, out {0});", name);
            }
            else if (type == typeof(string))
            {
                writer.WriteLine(tab + "{0} = reader.Read{1}();", name, type.Name);
            }
            else if (type.IsSubclassOf(typeof(IStaticData)))
            {
                writer.WriteLine(tab + "IStaticData {0};", name.Replace(".","").Replace("[", "").Replace("]", ""));
                writer.WriteLine(tab + "Read(reader, out {0}, {1});", name.Replace(".", "").Replace("[", "").Replace("]", ""), isEditorType ? "reader.ReadString() + \"" + StaticData.Generator.Constant.Suffix + "\"" : "reader.ReadString()");
                writer.WriteLine(tab + "{0} = {1} as {2};", name, name.Replace(".", "").Replace("[", "").Replace("]", ""), isEditorType ? type.Name : GetClassName(type));
            }
            else if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                writer.WriteLine(tab + "int {0}num = reader.ReadInt32();", name.Replace(".", "").Replace("[", "").Replace("]", ""));
                //writer.WriteLine(tab + "if( {0}num > 0 && {0}num < {1})", name.Replace(".", "").Replace("[", "").Replace("]", ""), StaticData.Generator.Constant.StaticDataUpLimit.ToString());
                writer.WriteLine(tab + "if( {0}num > 0 )", name.Replace(".", "").Replace("[", "").Replace("]", ""));
                writer.WriteLine(tab + "{");

                GenerateCSNewMethod(elementType, name, "", isEditorType, writer, tabNum + 1);

                writer.WriteLine(tab1 + "for(int {1}i = 0; {1}i < {1}num; {1}i ++)", name , name.Replace(".", "").Replace("[", "").Replace("]", ""));
                writer.WriteLine(tab1 + "{");
                GenerateCSBinaryReadMethod(elementType, name + "["+ name.Replace(".", "").Replace("[", "").Replace("]", "") + "i" + "]", isEditorType, structTypes, writer, tabNum + 2);
                writer.WriteLine(tab1 + "}");

                writer.WriteLine(tab + "}");
                //writer.WriteLine(tab + "else if({0}num >= {1} )", name.Replace(".", "").Replace("[", "").Replace("]", ""), StaticData.Generator.Constant.StaticDataUpLimit.ToString());
                //writer.WriteLine(tab + "{");
                //writer.WriteLine(tab1 + "StaticDataUtility.LogError(\"binary is bad!!! please rebuild it! 二进制文件已损坏,请重新生成!\");");
                //writer.WriteLine(tab + "}");
            }
        }
        private static void GenerateCSNewMethod(Type elementType, string name, string suffix, bool isEditorType, StreamWriter writer, int tabNum)
        {
            string tab = TabString(tabNum);

            if (elementType.IsEnum)
            {
                writer.WriteLine(tab + "{0} = new {1}[{2}num]{3};", name, elementType.FullName, name.Replace(".", "").Replace("[", "").Replace("]", ""), suffix);
            }
            else if(elementType.IsPrimitive || elementType == typeof(decimal))
            {
                writer.WriteLine(tab + "{0} = new {1}[{2}num]{3};", name, elementType.FullName, name.Replace(".", "").Replace("[", "").Replace("]", ""), suffix);
            }
            else if (elementType.IsValueType)
            {
                writer.WriteLine(tab + "{0} = new {1}[{2}num]{3};", name, elementType.FullName, name.Replace(".", "").Replace("[", "").Replace("]", ""), suffix);

            }
            else if (elementType == typeof(string))
            {
                writer.WriteLine(tab + "{0} = new {1}[{2}num]{3};", name, elementType.FullName, name.Replace(".", "").Replace("[", "").Replace("]", ""), suffix);

            }
            else if (elementType.IsSubclassOf(typeof(IStaticData)))
            {
                writer.WriteLine(tab + "{0} = new {1}[{2}num]{3};", name, isEditorType ? elementType.Name : GetClassName(elementType), name.Replace(".", "").Replace("[", "").Replace("]", ""), suffix);
            }
            else if (elementType.IsArray)
            {
                GenerateCSNewMethod(elementType.GetElementType(), name, suffix + "[]", isEditorType, writer, tabNum);
            }
        }
        public static void GenerateCSWriteMethod(string typeName, Type type, List<Type> structTypes, bool empty,  StreamWriter writer, int tabNum)
        {
            if (writer != null)
            {
                string tab = TabString(tabNum);
                string tabChild = tab + "\t";
                string child = "child";
                writer.WriteLine(tab + string.Format("private static void Write(XmlDocument doc, XmlNode father,{0} obj, string label = \"\")", typeName));
                writer.WriteLine(tab + "{");
                if (!empty)
                {
                    writer.WriteLine(tabChild + string.Format("XmlNode element = XmlSerializerUtility.CreateElement(doc, {0});", string.Format("string.IsNullOrEmpty(label)?\"{0}\":label.Replace(\"[\", \"\").Replace(\"]\", \"\")", GetClassLabel(type))));
                    writer.WriteLine(tabChild + "XmlSerializerUtility.AddAttribute(doc, element, \"Type\", StaticDataUtility.GetGenerateClassName(obj.GetType(), Constant.Suffix, false));", string.Format("\"{0}\"", StaticData.Generator.StaticDataUtility.GetGenerateClassName(type, StaticData.Generator.Constant.Suffix, false)));
                    writer.WriteLine(tabChild + string.Format("XmlSerializerUtility.AppendChild(father, element);"));
                    writer.WriteLine(tabChild + "XmlNode {0} = null;", child);
                    StaticData.Generator.StaticDataUtility.TranverseFields(type, (field) =>
                    {
                        GenerateCSWriteMethod(field.FieldType, "element", child, field.Name, structTypes, writer, tabNum);
                    });

                }
                writer.WriteLine(tab + "}");
            }
        }
        private static void GenerateCSWriteMethod(Type type, string father, string son, string name, List<Type> structTypes, StreamWriter writer, int tabNum)
        {
            string tab = TabString(tabNum);
            string tabChild = tab + "\t";
            string tabChild2 = tabChild + "\t";
            string tabChild3 = tabChild2 + "\t";

            //string arrayChildName = (name.IndexOf("[") > -1) ? GetClassName(type).Replace("[","_").Replace("]", "_") : name;
            string arrayChildName = (name.IndexOf("[") > -1) ? "element" : name;

            if (type.IsEnum)
            {
                writer.WriteLine(tabChild + "{0} = XmlSerializerUtility.CreateElement(doc, \"{1}\");", son, arrayChildName /*string.Format("\"{0}\".Replace(\"[\", \"\").Replace(\"]\", \"\")", name)*/);
                writer.WriteLine(tabChild + "XmlSerializerUtility.SetInnerText({0}, obj.{1}.ToString());", son, name);
                writer.WriteLine(tabChild + "XmlSerializerUtility.AppendChild({1}, {0});", son, father);
            }
            else if (type.IsPrimitive || type == typeof(decimal))
            {
                writer.WriteLine(string.Format(tabChild + "{0} = XmlSerializerUtility.CreateElement(doc, \"{1}\");", son, arrayChildName /*string.Format("\"{0}\".Replace(\"[\", \"\").Replace(\"]\", \"\")", name)*/));
                writer.WriteLine(string.Format(tabChild + "XmlSerializerUtility.SetInnerText({0}, obj.{1}.ToString());", son, name));
                writer.WriteLine(string.Format(tabChild + "XmlSerializerUtility.AppendChild({1}, {0});", son, father));
            }
            else if (type.IsValueType)
            {
                if(!structTypes.Contains(type))
                {
                    structTypes.Add(type);
                }
                writer.WriteLine(tabChild + "Write(doc, {1}, obj.{0}, \"{2}\");", name, father, arrayChildName);
            }
            else if (type.IsSubclassOf(typeof(IStaticData)))
            {
                writer.WriteLine(tabChild + "if(obj.{0} != null)", name);
                writer.WriteLine(tabChild + "{");
                writer.WriteLine(tabChild2 + "IStaticData data = obj.{0};", name);
                writer.WriteLine(tabChild2 + "Write(doc, {1}, data, \"{2}\");", name, father, arrayChildName);
                writer.WriteLine(tabChild + "}");
                writer.WriteLine(tabChild + "else");
                writer.WriteLine(tabChild + "{");
                writer.WriteLine(tabChild2 + "{0} = XmlSerializerUtility.CreateElement(doc, \"{1}\");", son, arrayChildName);
                writer.WriteLine(tabChild2 + "XmlSerializerUtility.AppendChild({1}, {0});", son, father);
                writer.WriteLine(tabChild + "}");
            }
            else if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                writer.WriteLine(tabChild + "if(obj.{0} != null)", name);
                writer.WriteLine(tabChild + "{");
                writer.WriteLine(tabChild2 + "{0} = XmlSerializerUtility.CreateElement(doc, \"{1}\");", son, arrayChildName);
                writer.WriteLine(tabChild2 + "XmlSerializerUtility.AppendChild({1}, {0});", son, father);
                writer.WriteLine(tabChild2 + "int {1}num = obj.{0}.Length;", name , name.Replace(".", "").Replace("[", "").Replace("]", ""));
                writer.WriteLine(tabChild2 + "for (int {0}i = 0; {0}i < {0}num; {0}i++)", name.Replace(".","").Replace("[", "").Replace("]", ""));
                writer.WriteLine(tabChild2 + "{");
                writer.WriteLine(tabChild3 + "XmlNode {0} = null;", son + son);
                string subName = name + "[" + name.Replace(".", "").Replace("[", "").Replace("]", "") + "i" + "]";
                GenerateCSWriteMethod(elementType, son, son + son, subName, structTypes, writer, tabNum + 2);

                writer.WriteLine(tabChild2 + "}");
                writer.WriteLine(tabChild + "}");
                writer.WriteLine(tabChild + "else");
                writer.WriteLine(tabChild + "{");
                writer.WriteLine(tabChild2 + "{0} = XmlSerializerUtility.CreateElement(doc, \"{1}\");", son, arrayChildName);
                writer.WriteLine(tabChild2 + "XmlSerializerUtility.AppendChild({1}, {0});", son, father);
                writer.WriteLine(tabChild + "}");


            }
            else if (type == typeof(string))
            {
                writer.WriteLine(string.Format(tabChild + "{0} = XmlSerializerUtility.CreateElement(doc, \"{1}\");", son, arrayChildName));
                writer.WriteLine(string.Format(tabChild + "XmlSerializerUtility.SetInnerText({0}, obj.{1});", son, name));
                writer.WriteLine(string.Format(tabChild + "XmlSerializerUtility.AppendChild({1}, {0});", son, father));
            }
        }
        public static void GenerateCSReadMethod(string typeName, Type type, List<Type> structTypes, bool empty,  StreamWriter writer, int tabNum)
        {
            if (writer != null)
            {
                string tab = TabString(tabNum);
                string tabChild = tab + "\t";
                string tabChild2 = tabChild + "\t";
                string tabChild3 = tabChild2 + "\t";

                writer.WriteLine(tab + string.Format("private static void Read(XmlNode node ,out {0} obj)", typeName));
                writer.WriteLine(tab + "{");
                if (type.IsClass)
                {
                    writer.WriteLine(tabChild + "obj = null;");
                }
                else
                {
                    writer.WriteLine(tabChild + "obj = new {0}();", typeName);
                }
                writer.WriteLine(tabChild + "string innerText = null;");
                if (!empty)
                {
                    writer.WriteLine(tabChild + "if (node != null)");
                    writer.WriteLine(tabChild + "{");
                    writer.WriteLine(tabChild2 + "XmlNode child = null;");
                    if (type.IsClass)
                    {
                        writer.WriteLine(tabChild2 + "if(obj == null)");
                        writer.WriteLine(tabChild2 + "{");
                        writer.WriteLine(tabChild3 + "obj = new {0}();", typeName);
                        writer.WriteLine(tabChild2 + "}");
                    }

                    StaticData.Generator.StaticDataUtility.TranverseFields(type, (field) =>
                    {
                        GenerateCSReadMethod(type, field,field.FieldType, field.Name, "node", "child", "\"" + field.Name + "\"", IsEditor(type, typeName), typeName, structTypes, writer, tabNum);
                    });

                    writer.WriteLine(tabChild + "}");
                }
                writer.WriteLine(tab + "}");
            }
        }
        private static void GenerateCSFieldReadMethod(Type classType, string nodeName, string son, string father, string name, Type type, string typeName, Action<string> action, Action<string> defaultAction, bool allowEmpty, StreamWriter writer, int tabNum)
        {
            string tabChild0 = TabString(tabNum);
            string tabChild1 = tabChild0 + "\t";
            string tabChild2 = tabChild1 + "\t";

            writer.WriteLine(tabChild0 + "{1} = XmlSerializerUtility.GetChildrenNode({2}, {0});", nodeName, son, father);
            writer.WriteLine(tabChild0 + "if({0} == null)", son);
            writer.WriteLine((tabChild0 + "{"));
            //writer.WriteLine(tabChild1 + "obj.{0} = {1};", name, GetCSDefaultValueString(type));
            if (defaultAction != null)
            {
                defaultAction(tabChild1);
            }
            writer.WriteLine(tabChild1 + "StaticDataUtility.{2}(\"类型为{0} 的字段 {1} 的XML数据不存在. {3});", typeName, name, "LogDesinerError", (classType.IsSubclassOf(typeof(StaticData.Template))) ? string.Format("ID = \" + obj.ID") : "\"");
            writer.WriteLine((tabChild0 + "}"));

            writer.WriteLine((tabChild0 + "else"));
            writer.WriteLine((tabChild0 + "{"));
            writer.WriteLine(tabChild1 + "innerText = XmlSerializerUtility.GetInnerText({0});", son);
            writer.WriteLine(tabChild1 + "if(string.IsNullOrEmpty(innerText))");
            writer.WriteLine((tabChild1 + "{"));
            if (defaultAction != null)
            {
                defaultAction(tabChild2);
            }
            if (allowEmpty)
            {
                //writer.WriteLine(tabChild2 + "StaticDataUtility.{2}(\"{0} : {1}'s empty.  ID = \");", typeName, name, "LogWarning");
            }
            else
            {
                writer.WriteLine(tabChild2 + "StaticDataUtility.{2}(\"类型为{0} 的字段 {1} 的XML数据里面是空的,没有写数据.  {3});", typeName, name, "LogDesinerError", (classType.IsSubclassOf(typeof(StaticData.Template))) ? string.Format("ID = \" + obj.ID") : "\"");
            }
            writer.WriteLine((tabChild1 + "}"));

            writer.WriteLine(tabChild1 + "else");
            writer.WriteLine(tabChild1 + "{");
            if(action != null)
            {
                action(tabChild2);
            }
            writer.WriteLine(tabChild1 + "}");
            writer.WriteLine(tabChild0 + "}");

        }
        private static void GenerateCSReadMethod(Type classType,  FieldInfo field, Type type, string name, string father, string son, string nodeName, bool isEditorType,string typeName, List<Type> structTypes , StreamWriter writer, int tabNum)
        {
            string tab = TabString(tabNum);
            string tabChild = tab + "\t";
            string tabChild2 = tabChild + "\t";
            string tabChild3 = tabChild2 + "\t";
            string tabChild4 = tabChild3 + "\t";
            string tabChild5 = tabChild4 + "\t";

            bool empty = StaticData.Generator.StaticDataUtility.HasAttribute<AllowEmptyAttribute>(field);
            DefaultValueAttribute attribute = StaticData.Generator.StaticDataUtility.GetAttribute<DefaultValueAttribute>(field);
            if (type.IsEnum)
            {
                GenerateCSFieldReadMethod(classType, nodeName, son, father, name, type, typeName, (t) =>
                {
                    writer.WriteLine(t + "if (!System.Enum.TryParse<{1}>(XmlSerializerUtility.GetInnerText({2}), out obj.{0}))", name, type.FullName, son);
                    writer.WriteLine(t + "{");
                    writer.WriteLine(t + "\t" + "StaticDataUtility.{2}(\"类型为{0} 的字段 {1} 的数据格式不对.  {3});", typeName, name, "LogDesinerError", (classType.IsSubclassOf(typeof(StaticData.Template))) ? string.Format("ID = \" + obj.ID") : "\"");
                    writer.WriteLine((t + "}"));
                }, (t)=> 
                {
                    if (attribute != null)
                    {
                        writer.WriteLine(t + "obj.{0} = {2}.{1};", name, attribute.GetValue(), field.FieldType.FullName);
                    }
                    else
                    {
                        writer.WriteLine(t + "obj.{0} = {1};", name, GetCSDefaultValueString(type));
                    }

                }, empty, writer, tabNum + 2);
            }
            else if (type.IsPrimitive || type == typeof(decimal))
            {
                GenerateCSFieldReadMethod(classType, nodeName, son, father, name, type, typeName, (t) =>
                {
                    writer.WriteLine(t + "if(!{0}.TryParse(innerText, out obj.{1}))", type.FullName, name);
                    writer.WriteLine(t + "{");
                    writer.WriteLine(t + "\t" + "StaticDataUtility.{2}(\"类型为{0} 的字段 {1} 的数据格式不对. {3});", typeName, name, "LogDesinerError", (classType.IsSubclassOf(typeof(StaticData.Template))) ? string.Format("ID = \" + obj.ID") : "\"");
                    writer.WriteLine(t + "}");
                }, (t)=> 
                {
                    if (attribute != null)
                    {
                        if (type == typeof(float) || type == typeof(double))
                        {
                            writer.WriteLine(t + "obj.{0} = {1}f;", name, attribute.GetValue());
                        }
                        else if(type == typeof(bool))
                        {
                            writer.WriteLine(t + "obj.{0} = {1};", name, attribute.GetValue().ToString().ToLower());
                        }
                        else
                        {
                            writer.WriteLine(t + "obj.{0} = {1};", name, attribute.GetValue());
                        }
                    }
                    else
                    {
                        writer.WriteLine(t + "obj.{0} = {1};", name, GetCSDefaultValueString(type));
                    }
                }, empty, writer, tabNum + 2);
            }
            else if (type.IsValueType)
            {
                if (!structTypes.Contains(type))
                {
                    structTypes.Add(type);
                }
                GenerateCSFieldReadMethod(classType, nodeName, son, father, name, type, typeName, (t) =>
                {
                    writer.WriteLine(tabChild4 + "Read({1} ,out obj.{0});", name, son);
                }, (t) =>
                {
                    if (attribute != null)
                    {
                        writer.WriteLine(t + "obj.{0} = {1};", name, attribute.GetValue());
                    }
                    else
                    {
                        writer.WriteLine(t + "obj.{0} = {1};", name, GetCSDefaultValueString(type));
                    }
                },  empty, writer, tabNum + 2);

            }
            else if (type.IsSubclassOf(typeof(IStaticData)))
            {
                GenerateCSFieldReadMethod(classType, nodeName, son, father, name, type, typeName, (t) =>
                {
                    writer.WriteLine(t + "if({0} != null && {0}.Attributes.Count > 0 && {0}.Attributes[\"Type\"] != null && !string.IsNullOrEmpty({0}.Attributes[\"Type\"].Value))", son);
                    writer.WriteLine(t + "{");

                    writer.WriteLine(t + "\t" + "IStaticData {0} = null;", "_" + name.Replace("[", "").Replace("]", "").Replace(".", ""));
                    writer.WriteLine(t + "\t" + "Read({1} ,out {0}, {1}.Attributes[\"Type\"].Value + \"{2}\");", "_" + name.Replace("[", "").Replace("]", "").Replace(".", ""), son, isEditorType ? StaticData.Generator.Constant.Suffix : "");
                    writer.WriteLine(t + "\t" + "obj.{0} = {1} as {2};", name, "_" + name.Replace("[", "").Replace("]", "").Replace(".", ""), isEditorType ? type.Name : GetClassName(type));

                    writer.WriteLine(t + "}");
                    writer.WriteLine(t + "else if({0} != null && {0}.Attributes.Count > 0 && {0}.Attributes[\"type\"] != null && !string.IsNullOrEmpty({0}.Attributes[\"type\"].Value))", son);
                    writer.WriteLine(t + "{");

                    writer.WriteLine(t + "\t" + "IStaticData {0} = null;", "_" + name.Replace("[", "").Replace("]", "").Replace(".", ""));
                    writer.WriteLine(t + "\t" + "Read({1} ,out {0}, {1}.Attributes[\"type\"].Value + \"{2}\");", "_" + name.Replace("[", "").Replace("]", "").Replace(".", ""), son, isEditorType ? StaticData.Generator.Constant.Suffix : "");
                    writer.WriteLine(t + "\t" + "obj.{0} = {1} as {2};", name, "_" + name.Replace("[", "").Replace("]", "").Replace(".", ""), isEditorType ? type.Name : GetClassName(type));

                    writer.WriteLine(t + "}");

                    writer.WriteLine(t + "else");
                    writer.WriteLine(t + "{");
                    writer.WriteLine(t + "\t" + "Read({1} , out obj.{0});", name, son);
                    writer.WriteLine(t + "}");

                },(t)=> 
                {
                    writer.WriteLine(t + "obj.{0} = {1};", name, GetCSDefaultValueString(type));
                    //writer.WriteLine(t + "obj.{0} = new {1}();", name, isEditorType ? type.Name : GetClassName(type));
                }, true, writer, tabNum + 2);

            }
            else if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                GenerateCSFieldReadMethod(classType, nodeName, son, father, name, type, typeName, (t) =>
                {
                    writer.WriteLine(tabChild4 + "int obj{1}num = XmlSerializerUtility.GetChildrenNum({0});", son, name.Replace(".", "").Replace("[", "").Replace("]", ""));
                    //writer.WriteLine(tabChild4 + "if(obj{1}num >= {0})", StaticData.Generator.Constant.StaticDataUpLimit.ToString(), name.Replace(".", "").Replace("[", "").Replace("]", ""));
                    //writer.WriteLine(tabChild4 + "{");
                    //writer.WriteLine(tabChild5 + "StaticDataUtility.LogDesinerError(\"{0}.{1}'s number is more then {2}. 超出数组最大长度{2}\");", typeName, name, StaticData.Generator.Constant.StaticDataUpLimit.ToString());

                    //writer.WriteLine((tabChild4 + "}"));
                    GenerateCSNewMethod(elementType, "obj." + name, "", isEditorType, writer, tabNum + 4);

                    writer.WriteLine(tabChild4 + "for(int {0}i = 0; {0}i < obj{0}num; {0}i ++)", name.Replace(".", "").Replace("[", "").Replace("]", ""));
                    writer.WriteLine(tabChild4 + "{");

                    writer.WriteLine(tabChild5 + "XmlNode {0}arrayChild = null;", name.Replace(".", "").Replace("[", "").Replace("]", ""));
                    GenerateCSReadMethod(classType, null, elementType, name + "[" + name.Replace(".", "").Replace("[", "").Replace("]", "") + "i" + "]", "child", name.Replace(".", "").Replace("[", "").Replace("]", "") + "arrayChild", name.Replace(".", "").Replace("[", "").Replace("]", "") + "i", isEditorType, typeName, structTypes, writer, tabNum + 3);
                    writer.WriteLine((tabChild4 + "}"));
                }, (t) =>
                {
                    writer.WriteLine(t + "obj.{0} = {1};", name, GetCSDefaultValueString(type));
                }, true, writer, tabNum + 2);
            }
            else if (type == typeof(string))
            {
                GenerateCSFieldReadMethod(classType, nodeName, son, father, name, type, typeName, (t) =>
                {
                    writer.WriteLine(string.Format(tabChild4 + "obj.{0} = innerText;", name));
                },(t)=> 
                {
                    if (attribute != null)
                    {
                        writer.WriteLine(t + "obj.{0} = \"{1}\";", name, attribute.GetValue());
                    }
                    else
                    {
                        writer.WriteLine(t + "obj.{0} = \"\";", name);
                    }
                }, true, writer, tabNum + 2);
            }
        }


        private static string GetCSDefaultValueString(Type type)
        {
            return StaticData.Generator.StaticDataUtility.GetCSDefaultValueString(type);
        }



        public static void GenerateStructBinaryReadSerialize(List<Type> list,bool empty, StreamWriter writer, int tabNum)
        {
            for (int i = 0; i < list.Count; i++)
            {
                GenerateCSBinaryReadMethod(list[i].Name, list[i], list, empty, writer, tabNum);
            }
        }
        public static void GenerateStructBinaryWriteSerialize(List<Type> list, bool empty,  StreamWriter writer, int tabNum)
        {
            for (int i = 0; i < list.Count; i++)
            {
                GenerateCSBinaryWriteMethod(list[i].Name, list[i], list, empty,  writer, tabNum);
            }
        }
        public static void GenerateStructXmlReadSerialize(List<Type> list,bool empty,  StreamWriter writer, int tabNum)
        {
            for (int i = 0; i < list.Count; i++)
            {
                GenerateCSReadMethod(list[i].Name, list[i], list, empty, writer, tabNum);
            }
        }
        public static void GenerateStructXmlWriteSerialize(List<Type> list,bool empty, StreamWriter writer, int tabNum)
        {
            for (int i = 0; i < list.Count; i++)
            {
                GenerateCSWriteMethod(list[i].Name, list[i], list, empty, writer, tabNum);
            }
        }

    }
}