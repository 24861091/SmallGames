/************************************************
 created : 2018.8
 author : caiming
************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using EchoEngine;
using UnityEngine;

namespace StaticData.Generator
{
    public static partial class StaticDataUtility
    {
        #region Log
        private static string LogInformation = null;
        public static void BeginLogRecord(string information)
        {
            LogInformation = information;
        }
        public static void EndLogRecord()
        {
            LogInformation = null;
        }
        public static void Log(string log)
        {
            if (LogInformation == null)
            {
                Facade.Instance.Log(log);
            }
            else
            {
                Facade.Instance.Log(LogInformation + log);
            }

        }

        public static void LogWarning(string log)
        {
            if (LogInformation == null)
            {
                Facade.Instance.LogWarning(log);
            }
            else
            {
                Facade.Instance.LogWarning(LogInformation + log);
            }
        }
        public static void LogErrorAndThrowException(string log)
        {
            LogError(log);
            //throw new Exception(log);
        }

        public static void LogError(string log)
        {
            if (LogInformation == null)
            {
                Facade.Instance.LogError(log);
            }
            else
            {
                Facade.Instance.LogError(LogInformation + log);
            }
        }
        public static void LogDesinerError(string log)
        {
            if (LogInformation == null)
            {
                Facade.Instance.LogDesinerError(log);
            }
            else
            {
                Facade.Instance.LogDesinerError(LogInformation + log);
            }
        }

        public static string LogInstance(object o, string name = "")
        {
            if (o != null)
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                Type type = o.GetType();
                FieldInfo[] fields = GetFieldsHierarchy(type);
                if (fields != null)
                {
                    for (int i = 0; i < fields.Length; i++)
                    {
                        FieldInfo field = fields[i];
                        if (field != null)
                        {
                            if (field.FieldType.IsEnum || field.FieldType.IsPrimitive || field.FieldType == typeof(decimal) || field.FieldType == typeof(string))
                            {
                                builder.Append(string.Format("{2}{0} = {1}\n", field.Name, field.GetValue(o), name));
                            }
                            else
                            {
                                builder.Append(LogInstance(field.GetValue(o), field.Name + "."));
                            }

                        }
                    }
                }
                return builder.ToString();
            }
            return string.Empty;
        }

        #endregion
        #region XML
#if RUNTIME_XML
        public static XmlNode GetBaseElement(XmlDocument doc)
        {
            XmlNode element = doc.FirstChild;
            while (element != null && element.Name != "StaticData")
            {
                element = element.NextSibling;
            }
            return element;
        }
        public static void TranverseElementXML(string dataName, Func<XmlElement,string, bool> action)
        {
            if(action == null)
            {
                return;
            }
            Stack<string> stack = new Stack<string>();
            for (int j = 0; j < Constant.XMLStaticDataPaths.Length; j++)
            {
                stack.Push(Constant.XMLStaticDataPaths[j]);
            }
            bool quit = false;
            while (stack.Count > 0)
            {
                if (quit)
                {
                    break;
                }

                string path = stack.Pop();
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
                if (path != null && Directory.Exists(path))
                {
                    string[] folders = Directory.GetDirectories(path);
                    if (folders != null)
                    {
                        for (int m = 0; m < folders.Length; m++)
                        {
                            stack.Push(folders[m]);
                        }
                    }
                    string[] files = Directory.GetFiles(path);
                    
                    for (int j = 0; j < files.Length; j++)
                    {
                        if(quit)
                        {
                            break;
                        }
                        string file = files[j];
                        string fileName = Path.GetFileName(file);
                        if (fileName.IndexOf(dataName) > -1)
                        {
                            XmlDocument doc = new XmlDocument();
                            XmlElement firstChild = null;
                            doc.Load(file);
                            firstChild = StaticData.StaticDataUtility.GetBaseElement(doc) as XmlElement;
                            if (firstChild == null)
                            {
                                StaticDataUtility.LogError(string.Format("{0} 是空的.", file));
                            }
                            else if (firstChild.HasChildNodes)
                            {
                                int n = firstChild.ChildNodes.Count;
                                for (int i = 0; i < n; i++)
                                {
                                    if (quit)
                                    {
                                        break;
                                    }

                                    XmlNode template = firstChild.ChildNodes[i];
                                    if (template.Attributes.Count <= 0)
                                    {
                                        continue;
                                    }
                                    if(template.Attributes["Type"] == null && template.Attributes["type"] == null)
                                    {
                                        continue;
                                    }
                                    string v = null;
                                    if(template.Attributes["Type"] != null)
                                    {
                                        v = template.Attributes["Type"].Value;
                                    }
                                    else 
                                    {
                                        v = template.Attributes["type"].Value;
                                    }
                                    if (v != dataName && v != "StaticData." + dataName)
                                    {
                                        continue;
                                    }
                                    XmlElement element = template as XmlElement;
                                    if (element != null)
                                    {
                                        if(action(element, fileName))
                                        {
                                            quit = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            doc = null;
                            firstChild = null;
                        }
                    }
                }
            }


        }
#endif
        #endregion

        #region 生成代码会调用
        public static string GetGenerateClassName(Type editorTypeDefinationClass, string suffix, bool nameSpace = true)
        {
            string name = editorTypeDefinationClass.Name;
            if (nameSpace)
            {
                name = editorTypeDefinationClass.FullName;
            }
            return GetGenerateClassName(name, suffix);
        }
        public static string GetGenerateClassName(string editorTypeDefinationClassName, string suffix)
        {
            return editorTypeDefinationClassName.Replace(suffix, "");
        }
        public static string GetGenerateClassName(string editorTypeDefinationClassName)
        {
            return GetGenerateClassName(editorTypeDefinationClassName, Constant.Suffix);
        }
        #endregion
        //public static string GetDeclareValueTypeName(Type editorTypeDefinationClass)
        //{
        //    switch(editorTypeDefinationClass.FullName)
        //    {
        //        case "System.Boolean":
        //            return "bool";
        //        case "System.Char":
        //            return "char";
        //        case "System.String":
        //            return "string";
        //        case "System.Decimal":
        //            return "decimal";
        //        case "System.Byte":
        //            return "byte";
        //        case "System.Double":
        //            return "double";
        //        case "System.Single":
        //            return "float";
        //        case "System.Int32":
        //            return "int";
        //        case "System.Int64":
        //            return "long";
        //        case "System.SByte":
        //            return "sbyte";
        //        case "System.Int16":
        //            return "short";
        //        case "System.UInt32":
        //            return "uint";

        //        case "System.UInt64":
        //            return "ulong";
        //        case "System.UInt16":
        //            return "ushort";
        //    }
        //    return "";


        //}
        #region 和生成代码有关
        public static string TabString(int tabNum)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            for (int i = 0; i < tabNum; i++)
            {
                builder.Append("\t");
            }
            return builder.ToString();
        }
        public static string GetStaticDataTypeLabel(Type type)
        {
            if (type.IsSubclassOf(typeof(StaticData.Template)))
            {
                return "Template";
            }
            else if (type.IsSubclassOf(typeof(StaticData.Config)))
            {
                return "Config";
            }
            else if (type.IsSubclassOf(typeof(StaticData.IStaticData)))
            {
                return "StaticData";
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion


        #region XML operation
        public static XmlElement GetSubElement(XmlElement element, string name)
        {
            if (element == null)
            {
                return null;
            }
            if (!element.HasChildNodes)
            {
                return null;
            }

            int n = element.ChildNodes.Count;
            for (int i = 0; i < n; i++)
            {
                if (element.ChildNodes[i].Name == name)
                {
                    return element.ChildNodes[i] as XmlElement;
                }
            }
            return null;
        }
        public static XmlElement GetSubElement(XmlElement element, int index)
        {
            if (element == null)
            {
                return null;
            }
            if (!element.HasChildNodes)
            {
                return null;
            }

            int n = element.ChildNodes.Count;
            if (index >= 0 && index < n)
            {
                return element.ChildNodes[index] as XmlElement;
            }
            return null;
        }

        public static XmlElement[] GetSubElements(XmlElement element, string name)
        {
            if (element == null)
            {
                return null;
            }
            if (!element.HasChildNodes)
            {
                return null;
            }
            List<XmlElement> list = new List<XmlElement>();
            int n = element.ChildNodes.Count;
            for (int i = 0; i < n; i++)
            {
                if (element.ChildNodes[i].Name == name)
                {
                    list.Add(element.ChildNodes[i] as XmlElement);
                }
            }
            return list.ToArray();
        }

        public static FieldInfo[] GetFieldsHierarchy(Type type)
        {
            return type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        }
        public static FieldInfo[] GetFieldsSelf(Type type)
        {
            return type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        }
        #endregion

        #region default string
        public static string GetCSEnumDefaultValueString(Type type)
        {
            return string.Format("default({0})", type.FullName);
        }
        public static string GetCSDefaultValueString(Type type)
        {
            if (type.IsEnum)
            {
                return GetCSEnumDefaultValueString(type);
            }
            else if (!type.IsValueType)
            {
                return "null";
            }
            return GetCSDefaultValueString(type.FullName);
        }
        public static string GetCSDefaultValueString(string typeName)
        {
            switch (typeName)
            {
                case "System.Boolean":
                    return "false";
                case "System.Double":
                case "System.Single":
                    return "0f";
                case "System.Char":
                    return @"'\0'";
                case "System.Decimal":
                case "System.Byte":
                case "System.Int32":
                case "System.Int64":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.UInt16":
                    return "0";
                case "System.String":
                    return "\"\"";
                default:
                    return string.Format("default({0})", typeName);
            }

        }
        public static string GetLuaDefaultValueString(Type type)
        {
            return GetLuaDefaultValueString(type.FullName);
        }
        private static string GetLuaDefaultValueString(string typeName)
        {
            switch (typeName)
            {
                case "System.Boolean":
                    return "false";
                case "System.Char":
                    return "nil";
                case "System.Double":
                case "System.Single":
                case "System.Decimal":
                case "System.Byte":
                case "System.Int32":
                case "System.Int64":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.UInt16":
                    return "0";
                case "System.String":
                    return "\"\"";
                default:
                    return "nil";
            }


        }

        public static string GetSerializeDefaultValueString(Type type)
        {
            return GetSerializeDefaultValueString(type.FullName);
        }
        public static string GetSerializeDefaultValueString(string typeName)
        {
            switch (typeName)
            {
                case "System.Boolean":
                    return "false";
                case "System.Double":
                case "System.Single":
                case "System.Decimal":
                case "System.Byte":
                case "System.Int32":
                case "System.Int64":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.UInt16":
                    return "0";
                case "System.String":
                    return "\"\"";

                case "System.Char":
                default:
                    return "nil";
            }

        }
        #endregion
        public static void ResortID(string idFieldName, FieldInfo[] infos)
        {
            if (infos != null && infos.Length > 1)
            {
                int index = -1;
                for (int i = 0; i < infos.Length; i++)
                {
                    FieldInfo field = infos[i];
                    if (field != null)
                    {
                        if (field.Name == idFieldName)
                        {
                            index = i;
                            break;
                        }
                    }
                }

                if (index > 0 && index < infos.Length)
                {
                    FieldInfo field = infos[index];

                    for (int j = index; j >= 1; j--)
                    {
                        infos[j] = infos[j - 1];
                    }
                    infos[0] = field;
                }
            }
        }
        public static string Key(IStaticData data)
        {
            if (data != null)
            {
                string typeName = data.GetType().Name;
                Template template = data as Template;
                int index = -1;
                if (template != null)
                {
                    index = template.ID;
                    return typeName + index.ToString();
                }
                else
                {
                    return typeName;
                }
            }
            return string.Empty;
        }

        public static void TranverseFields(Type type, Action<FieldInfo> action)
        {
            if (action == null)
            {
                return;
            }
            FieldInfo[] fields = StaticDataUtility.GetFieldsHierarchy(type);
            StaticDataUtility.ResortID("ID", fields);
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    if (field != null)
                    {
                        action(field);
                    }
                }
            }

        }

        public static bool HasAttribute<AttributeType>(FieldInfo field) where AttributeType : Attribute
        {
            if (field != null)
            {
                IEnumerable<CustomAttributeData> attributes = field.CustomAttributes;
                if (attributes != null)
                {
                    IEnumerator<CustomAttributeData> enumerator = attributes.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        CustomAttributeData data = enumerator.Current as CustomAttributeData;
                        if (data != null && data.AttributeType == typeof(AttributeType))
                        {
                            return true;
                        }
                    }
                }

            }
            return false;
        }
        public static AttributeType GetAttribute<AttributeType>(FieldInfo field) where AttributeType : Attribute
        {
            if (field == null)
            {
                return null;
            }
            return field.GetCustomAttribute<AttributeType>();
        }

    }
}