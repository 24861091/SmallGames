using StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace StaticDataEditor
{
    public static partial class StaticDataUtilityEditor 
    {
        public static string CurFilePath;

        public static void LoadFile(string file, string path, Dictionary<string, IStaticData> datas, string suffix = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(file) && file.ToLower().EndsWith(".xml"))
                {
                    string filePath = Path.Combine(path, file);
                    if (File.Exists(filePath))
                    {
                        CurFilePath = filePath;
                        XmlDocument doc = new XmlDocument();
                        doc.Load(filePath);
                        XmlNode child = GetBaseElement(doc);
                        if (child == null)
                        {
                            StaticDataUtility.LogError(string.Format("{0} 是空的.", file));
                        }
                        int n = child.ChildNodes.Count;
                        for (int m = 0; m < n; m++)
                        {
                            XmlNode template = child.ChildNodes[m];
                            if (template != null)
                            {
                                StaticDataUtility.BeginLogRecord(string.Format("文件路径 {0} 类型是 {1} ", filePath, template.Attributes[0].Value));
                                string v = null;
                                LoadStaticData(template, out v);
                                IStaticData value = null;
                                if (!string.IsNullOrEmpty(v))
                                {
                                    StaticDataEditor.FacadeEditor.Instance.Read(template, out value, v + suffix);
                                }
                                else
                                {
                                    StaticDataUtility.LogDesinerError("XML中 Attribute Type 没有写!");
                                }
                                if (value != null)
                                {
                                    string key = StaticData.Generator.StaticDataUtility.Key(value);
                                    

                                    //IStaticData find = list.Find((data) =>
                                    //{
                                    //    if(data != null)
                                    //    {
                                    //        return StaticDataUtility.Key(value) == StaticDataUtility.Key(data);
                                    //    }
                                    //    return false;
                                    //});
                                    if (!datas.ContainsKey(key) || datas[key] == null)
                                    {
                                        datas[key] = (value);
                                    }
                                    else
                                    {
                                        StaticDataUtility.LogDesinerError("配置的XML重复了! 名称:" + StaticData.Generator.StaticDataUtility.Key(value).ToString());
                                    }
                                }
                                StaticDataUtility.EndLogRecord();
                            }
                        }
                        doc = null;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticDataUtility.LogDesinerError("Load File Error : " + CurFilePath);
                StaticDataUtility.LogDesinerError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                StaticDataUtility.EndLogRecord();
            }
        }
        public static bool LoadStaticData(XmlNode template, out string v)
        {
            v = string.Empty;
            if (template.Attributes.Count <= 0)
            {
                return false;
            }
            if (template.Attributes["Type"] == null && template.Attributes["type"] == null)
            {
                return false;
            }
            if (template.Attributes["Type"] != null)
            {
                v = template.Attributes["Type"].Value;
            }
            else
            {
                v = template.Attributes["type"].Value;
            }
            return true;
        }


        public static IStaticData[] LoadXML(out string lastFile , string suffix = "")
        {
            lastFile = "";
            //List<IStaticData> list = new List<IStaticData>();
            Dictionary<string, IStaticData> datas = new Dictionary<string, IStaticData>();

            Stack<string> stack = new Stack<string>();
            for (int j = 0; j < StaticDataEditor.FacadeEditor.Instance.XMLStaticDataPaths.Length; j++)
            {
                stack.Push(StaticDataEditor.FacadeEditor.Instance.XMLStaticDataPaths[j]);
            }
            while (stack.Count > 0)
            {
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
                        string file = files[j];
                        lastFile = file;
                        LoadFile(file, path, datas, suffix);
                    }
                }
            }
            if(datas.Count <= 0)
            {
                StaticData.StaticDataUtility.LogError("没有读取到XML，请检查路径");
            }
            return new List<IStaticData>(datas.Values).ToArray();
        }

        public static TYPE Clone<TYPE>(TYPE instance) where TYPE : IStaticData
        {
            if(instance == null)
            {
                return null;
            }
            Type type = instance.GetType();

            TYPE clone =  Facade.Instance.Create(type) as TYPE;
            if (clone != null)
            {
                FieldInfo[] infos = StaticData.Generator.StaticDataUtility.GetFieldsHierarchy(type);
                for(int i = 0; i < infos.Length; i ++)
                {
                    FieldInfo field = infos[i];
                    if(field != null)
                    {
                        if(field.FieldType.IsEnum)
                        {
                            field.SetValue(clone, field.GetValue(instance));
                        }
                        else if(field.FieldType.IsPrimitive)
                        {
                            field.SetValue(clone, field.GetValue(instance));
                        }
                        else if(field.FieldType.IsValueType)
                        {
                            field.SetValue(clone, field.GetValue(instance));
                        }
                        else if(field.FieldType == typeof(string))
                        {
                            field.SetValue(clone, field.GetValue(instance));
                        }
                        else if (field.FieldType.IsSubclassOf(typeof(IStaticData)))
                        {
                            field.SetValue(clone, Clone(field.GetValue(instance) as IStaticData));
                        }
                        else if (field.FieldType.IsArray)
                        {
                            Array instanceArray = field.GetValue(instance) as Array;
                            if(instanceArray != null)
                            {
                                int n = instanceArray.Length;
                                Array array = Activator.CreateInstance(field.FieldType, n) as Array;
                                field.SetValue(clone, array);

                                for (int j = 0; j < n; j++)
                                {
                                    if (field.FieldType.GetElementType().IsEnum)
                                    {
                                        array.SetValue(instanceArray.GetValue(j), j);
                                    }
                                    else if (field.FieldType.GetElementType().IsPrimitive)
                                    {
                                        array.SetValue(instanceArray.GetValue(j), j);
                                    }
                                    else if (field.FieldType.GetElementType().IsValueType)
                                    {
                                        array.SetValue(instanceArray.GetValue(j), j);
                                    }
                                    else if (field.FieldType.GetElementType() == typeof(string))
                                    {
                                        array.SetValue(instanceArray.GetValue(j), j);
                                    }
                                    else if (field.FieldType.GetElementType().IsSubclassOf(typeof(IStaticData)))
                                    {
                                        array.SetValue(Clone(instanceArray.GetValue(j) as IStaticData), j);
                                    }
                                }

                            }
                        }
                    }
                }
            }

            return clone;
        }



        /// <summary>
        /// 该接口会用xml的新数据覆盖掉TemplateManager内的数据，暂时只给编辑器使用。
        /// </summary>
        public static KeyValuePair<string, TValue[]>[] LoadXMLPathForEditor<TValue>(string xmlPath, bool topDirectoryOnly = false) where TValue : IStaticData
        {
            List<KeyValuePair<string, TValue[]>> list = new List<KeyValuePair<string, TValue[]>>();
            Stack<string> stack = new Stack<string>();

            stack.Push(xmlPath);

            while (stack.Count > 0)
            {
                string path = stack.Pop();

                if (path != null && Directory.Exists(path))
                {
                    if (!topDirectoryOnly)
                    {
                        string[] folders = Directory.GetDirectories(path);
                        if (folders != null)
                        {
                            for (int m = 0; m < folders.Length; m++)
                            {
                                stack.Push(folders[m]);
                            }
                        }
                    }
                    string[] files = Directory.GetFiles(path);
                    for (int j = 0; j < files.Length; j++)
                    {
                        string file = files[j];
                        TValue[] values = LoadFileForEditor<TValue>(file, path);
                        if (values != null && values.Length > 0)
                        {
                            list.Add(new KeyValuePair<string, TValue[]>(file, values));
                        }

                    }
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 该接口会用xml的新数据覆盖掉TemplateManager内的数据，暂时只给编辑器使用。
        /// </summary>
        public static TValue[] LoadFileForEditor<TValue>(string file, string path) where TValue : IStaticData
        {
            List<TValue> list = new List<TValue>();
            Assembly assembly = Assembly.GetAssembly(typeof(StaticData.IStaticData));
            
            try
            {
                if (!string.IsNullOrEmpty(file))
                {
                    string filePath = Path.Combine(path, file);
                    if (File.Exists(filePath))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(filePath);
                        XmlNode first = GetBaseElement(doc);
                        int n = first.ChildNodes.Count;
                        for (int m = 0; m < n; m++)
                        {
                            XmlNode node = first.ChildNodes[m];
                            if (node.Attributes.Count > 0 /*&& node.Attributes[0].Value == typeof(TValue).Name*/)
                            {
                                string typeName = node.Attributes[0].Value;
                                string editorTypeName = typeName + StaticData.Constant.Suffix;
                                IStaticData value = null;/*StaticDataSummary.Create(typeof(TValue)) as TValue;*/
                                StaticData.Facade.Instance.Read(node, out value, typeName);
                                //StaticDataEditor.FacadeEditor.Instance.Read(node, out value, editorTypeName);
                                //var ins = assembly.CreateInstance("StaticData." + typeName);
                                //SetValue(value, ins);
                                //value = ins as IStaticData;
                                TValue tv = value as TValue;
                                if (tv != null)
                                {
                                    list.Add(tv);

                                    var tp = value as Template;
                                    if (tp != null)
                                    {
                                        TemplateManager.Instance.Remove(tp.GetType(), tp.ID);
                                        TemplateManager.Instance.Add(tp);
                                    }
                                }
                            }
                        }
                        doc = null;
                    }
                }
                return list.ToArray();
            }
            catch (Exception ex)
            {
                StaticData.StaticDataUtility.LogError("file Name = " + file + "\n" + ex.Message + "\n" + ex.StackTrace);
                return list.ToArray();
            }
        }
        public static XmlNode GetBaseElement(XmlDocument doc)
        {
            XmlNode element = doc.FirstChild;
            while (element != null && element.Name != "StaticData")
            {
                element = element.NextSibling;
            }
            return element;
        }
        public static void CreateXmlFileHead(out XmlDocument doc, out XmlDeclaration declaration, out XmlElement dataRoot)
        {
            doc = new XmlDocument();
            declaration = doc.CreateXmlDeclaration("1.0", "utf-8", "");
            doc.AppendChild(declaration);
            dataRoot = doc.CreateElement("StaticData");
            doc.AppendChild(dataRoot);
        }

        public static void Write(IStaticData[] datas, string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                StaticDataUtility.LogErrorAndThrowException(string.Format("路径不对 {0}", fullPath));
            }
            if (datas == null)
            {
                return;
            }
            XmlDocument doc;
            XmlDeclaration declaration;
            XmlElement root;
            CreateXmlFileHead(out doc, out declaration, out root);
            for (int i = 0; i < datas.Length; i++)
            {
                if (datas[i] != null)
                {
                    StaticData.Facade.Instance.Write(doc, root, datas[i]);
                }
            }
            doc.Save(fullPath);
        }


        public static void SetValue(object fromObject, object targetObject)
        {
            Type editorType = fromObject.GetType();
            FieldInfo[] editorFields = StaticData.Generator.StaticDataUtility.GetFieldsHierarchy(editorType);
            FieldInfo[] fields = StaticData.Generator.StaticDataUtility.GetFieldsHierarchy(targetObject.GetType());
            if (editorFields != null)
            {
                for (int i = 0; i < editorFields.Length; i++)
                {
                    FieldInfo editorField = editorFields[i];
                    FieldInfo field = fields[i];
                    if(editorField.FieldType.Name.Contains(StaticData.Constant.Suffix))
                    {
                        var target = field.GetValue(targetObject);
                        if (target != null)
                        {
                            SetValue(editorField.GetValue(fromObject), target);
                        }
                        
                    }
                    else
                    {
                        if (editorField != null && field != null)
                        {
                            field.SetValue(targetObject, editorField.GetValue(fromObject));
                        }
                    }
                }
            }
        }


        public static ExportTarget GetExportTarget(Type type)
        {
            return StaticDataEditor.Generator.StaticDataUtilityEditor.GetExportTarget(type);
        }

        private static void MatchStaticData(XmlNode node, Type type, StreamWriter writer)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                XmlNode child = node.ChildNodes[i];
                if(child.NodeType == XmlNodeType.Comment)
                {
                    writer.WriteLine("child 是注释 node.Name = {0} i = {1}", child.Name, i);
                    continue;
                }
                if(child == null)
                {
                    writer.WriteLine("child = null node.Name = {0} i = {1}", node.Name, i);
                    return;
                }

                FieldInfo nodeField = null;
                FieldInfo[] fields = StaticData.Generator.StaticDataUtility.GetFieldsHierarchy(type);
                if (fields == null)
                {
                    writer.WriteLine("common 类型没有字段 {0}", type.Name);
                    return;
                }
                for (int j = 0; j < fields.Length; j++)
                {
                    FieldInfo field = fields[j];
                    if (field.Name.ToLower().Replace("_", "") == child.Name.ToLower().Replace("_", ""))
                    {
                        nodeField = field;
                        break;
                    }
                }
                if (nodeField == null)
                {
                    writer.WriteLine("nodeField 没有找到 node.Name = " + child.Name);
                    continue;
                }


                Match(child, nodeField.FieldType, nodeField.Name, writer);
            }

        }
        private static void Match(XmlNode child, Type fieldType, string fieldName,  StreamWriter writer)
        {
            if (fieldName != child.Name)
            {
                XmlNode clone = child.OwnerDocument.CreateElement(fieldName);
                clone.InnerXml = child.InnerXml;
                if(child.Attributes != null)
                {
                    for (int i = 0; i < child.Attributes.Count; i++)
                    {
                        clone.Attributes.Append(child.Attributes[i]);
                    }
                }
                child.ParentNode.InsertAfter(clone, child);
                child.ParentNode.RemoveChild(child);
                child = clone;
            }

            if (child.ChildNodes.Count > 0 && child.Attributes.Count <= 0 && fieldType.IsArray)
            {
                //array
                Type elementType = fieldType.GetElementType();
                for (int j = 0; j < child.ChildNodes.Count; j++)
                {
                    Match(child.ChildNodes[j], elementType, elementType.Name, writer);
                }
            }
            else if (fieldType.IsSubclassOf(typeof(IStaticData)) && child.ChildNodes.Count > 0)
            {
                //IStaticData
                if (child.Attributes.Count > 0)
                {
                    string typeName = child.Attributes[0].Value;
                    Assembly assembly = Assembly.GetAssembly(typeof(StaticData.ConfigTestEditor));
                    Type type = assembly.GetType(string.Format("{0}", typeName));
                    if(type == null)
                    {
                        assembly = Assembly.GetAssembly(typeof(StaticData.ConfigTestEditor));
                        type = assembly.GetType(string.Format("StaticData.{0}{1}", typeName, StaticData.Generator.Constant.Suffix));
                    }

                    if (type == null)
                    {
                        writer.WriteLine("类型找不到{0}", typeName);
                        MatchStaticData(child, fieldType, writer);
                    }
                    else
                    {
                        MatchStaticData(child, type, writer);
                    }
                }
                else
                {
                    MatchStaticData(child, fieldType, writer);
                }
            }

        }



        //private static void Match(XmlNode child, Type type, StreamWriter writer)
        //{

        //    if(child != null)
        //    {
        //        FieldInfo nodeField = null;
        //        FieldInfo[] fields = StaticData.Generator.StaticDataUtility.GetFieldsHierarchy(type);
        //        if (fields == null)
        //        {
        //            writer.WriteLine("common 类型没有字段 {0}", type.Name);
        //            return;
        //        }
        //        for (int i = 0; i < fields.Length; i++)
        //        {
        //            FieldInfo field = fields[i];
        //            if (field.Name.ToLower().Replace("_", "") == child.Name.ToLower().Replace("_", ""))
        //            {
        //                nodeField = field;
        //                break;
        //            }
        //        }
        //        if(nodeField == null)
        //        {
        //            writer.WriteLine("nodeField 没有找到 node.Name = " + child.Name);
        //            return;
        //        }
        //        if (child.HasChildNodes && child.Attributes.Count <= 0)
        //        {
        //            //array
        //            if(!nodeField.FieldType.IsArray)
        //            {
        //                writer.WriteLine("node.Name = {0} 不是数组被认为是数组 ", child.Name);
        //                return;
        //            }
        //            Type elementType = nodeField.FieldType.GetElementType();
        //            for(int i = 0; i < child.ChildNodes.Count; i ++)
        //            {
        //                XmlNode child = child.ChildNodes[i];
        //                Match(child, elementType, writer);
        //            }
        //        }
        //        else if(child.HasChildNodes && child.Attributes.Count >= 0)
        //        {
        //            //IStaticData

        //        }
        //        else
        //        {
        //            //common
        //            child.InnerXml = Regex.Replace(child.InnerXml, string.Format(@"<\s*{0}\s*>", child.Name),(match)=> 
        //            {
        //                return string.Format("<{0}>", nodeField.Name);
        //            });

        //            child.InnerXml = Regex.Replace(child.InnerXml, string.Format(@"<\s*/\s*{0}\s*>", child.Name), (match) =>
        //            {
        //                return string.Format("</{0}>", nodeField.Name);
        //            });
        //        }
        //    }
        //}

        [MenuItem(@"Tools/刷新XML字段大小写")]
        public static void MakeXmlDefinationSameWithCode()
        {
            StreamWriter writer = File.CreateText("_log.txt");
            try
            {

                Assembly assembly = Assembly.GetAssembly(typeof(StaticData.ConfigTestEditor));
                if (assembly == null)
                {
                    writer.WriteLine("runtime 编译未过，待编译通过再执行");
                    writer.Close();
                    writer = null;
                    return;
                }

                string[] paths = FacadeEditor.Instance.XMLStaticDataPaths;
                if (paths == null)
                {
                    writer.WriteLine("FacadeEditor.Instance.XMLStaticDataPaths 不对");
                    writer.Close();
                    writer = null;
                    return;
                }
                Stack<string> stack = new Stack<string>();
                for (int i = 0; i < paths.Length; i++)
                {
                    string xmlPath = paths[i];
                    if (string.IsNullOrEmpty(xmlPath))
                    {
                        writer.WriteLine("第{0}个 xml的路径是空的", i);
                        continue;
                    }

                    stack.Push(xmlPath);
                }

                while (true)
                {
                    if (stack.Count <= 0)
                    {
                        break;
                    }
                    string root = stack.Pop();
                    if (string.IsNullOrEmpty(root))
                    {
                        writer.WriteLine("遇到错误目录名");
                        continue;
                    }
                    string[] folders = Directory.GetDirectories(root);
                    string[] files = Directory.GetFiles(root);

                    if (folders != null)
                    {
                        for (int i = 0; i < folders.Length; i++)
                        {
                            stack.Push(folders[i]);
                        }
                    }
                    if (files != null)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            string file = files[i];
                            if (string.IsNullOrEmpty(file))
                            {
                                writer.WriteLine("遇到错误文件名");
                                continue;
                            }
                            if (!File.Exists(file))
                            {
                                writer.WriteLine("文件不存在{0}", file);
                                continue;
                            }
                            writer.WriteLine("处理文件开始 {0}", file);

                            XmlDocument doc = new XmlDocument();
                            try
                            {
                                doc.Load(file);
                                XmlNode first = GetBaseElement(doc);
                                int n = first.ChildNodes.Count;
                                for (int m = 0; m < n; m++)
                                {
                                    XmlNode node = first.ChildNodes[m];
                                    if (node.Attributes.Count <= 0 || (string.IsNullOrEmpty(node.Attributes[0].Value)))
                                    {
                                        writer.WriteLine("文件内无数据类型{0}", file);
                                        continue;
                                    }
                                    string typeName = node.Attributes[0].Value;
                                    Type type = assembly.GetType(string.Format("StaticData.{0}{1}", typeName, StaticData.Constant.Suffix));
                                    if (type == null)
                                    {
                                        writer.WriteLine("类型找不到{0}", typeName);
                                        continue;
                                    }

                                    MatchStaticData(node, type, writer);
                                }

                            }
                            catch (Exception ex)
                            {
                                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
                            }
                            finally
                            {
                                doc.Save(file);
                                writer.WriteLine("处理文件结束 {0}", file);
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + @"\n" + ex.StackTrace);
            }
            finally
            {
                writer.WriteLine("程序结束");
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }

            
            
        }
    }

}