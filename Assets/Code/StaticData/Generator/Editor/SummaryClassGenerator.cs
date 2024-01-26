using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


namespace StaticDataEditor
{
    public class SummaryClassGenerator
    {
        private static string TabString(int num)
        {
            return StaticData.Generator.StaticDataUtility.TabString(num);
        }

        private static void GenerateDefination(StreamWriter writer, int tabNum)
        {
            if (writer != null)
            {
                string tab = TabString(tabNum);
                writer.WriteLine(tab + "using System.Collections;");
                writer.WriteLine(tab + "using System.Collections.Generic;");
                writer.WriteLine(tab + "using UnityEngine;");

                writer.WriteLine(tab + "using System.IO;");
                writer.WriteLine(tab + "using System.Xml;");
            }
        }

        public static void GenerateCSStaticNameArray(string arrayName, IList array, StreamWriter writer, int tabNum)
        {
            if (writer != null && array != null && !string.IsNullOrEmpty(arrayName))
            {

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < array.Count; i++)
                {
                    if (i == 0)
                    {
                        builder.Append("\"");
                    }
                    else
                    {
                        builder.Append(",\"");
                    }
                    builder.Append(array[i]);
                    builder.Append("\"");
                }
                writer.WriteLine(string.Format(StaticData.Generator.StaticDataUtility.TabString(tabNum) + "public static string[] {0} = new string[]{1}", arrayName, "{ " + builder.ToString() + " };"));
            }
        }

        public static void GenerateVersion(StreamWriter writer, int tabNum)
        {
            string tab = StaticData.Generator.StaticDataUtility.TabString(tabNum);
            writer.WriteLine(tab + string.Format("public static string Version = \"{0}\";", System.Guid.NewGuid().ToString()));
        }

        public static void GenerateCSSummaryClass(List<string> datas, bool empty, StreamWriter writer, int tabNum)
        {
            if (writer != null)
            {
                GenerateCSSummaryCreateMethod(datas, empty, writer, tabNum);
                if(Constant.canGenerateRuntimeBinaryRead)
                {
                    GenerateCSSummaryBinaryReadMethod(datas, empty, writer, tabNum);
                }
                if(Constant.canGenerateRuntimeBinaryWrite)
                {
                    GenerateCSSummaryBinaryWriteMethod(datas, empty, writer, tabNum);
                }
                if(Constant.canGenerateRuntimeXmlRead)
                {
                    GenerateCSSummaryReadMethod(datas, empty, writer, tabNum);
                }
                if(Constant.canGenerateRuntimeXmlWrite)
                {
                    GenerateCSSummaryWriteMethod(datas, empty, writer, tabNum);
                }

                GenerateCSSummaryGetTypeMethod(datas, empty, writer, tabNum);
            }
        }
        public static void GenerateCSSummaryEditorClass(List<string> datas, bool empty, StreamWriter writer, int tabNum)
        {
            if (writer != null)
            {
                GenerateCSSummaryCreateMethod(datas, empty, writer, tabNum);
                if (Constant.canGenerateEditorBinaryRead)
                {
                    GenerateCSSummaryBinaryReadMethod(datas, empty, writer, tabNum);
                }
                if (Constant.canGenerateEditorBinaryWrite)
                {
                    GenerateCSSummaryBinaryWriteMethod(datas, empty, writer, tabNum);
                }
                if (Constant.canGenerateEditorXmlRead)
                {
                    GenerateCSSummaryReadMethod(datas, empty, writer, tabNum);
                }
                if (Constant.canGenerateEditorXmlWrite)
                {
                    GenerateCSSummaryWriteMethod(datas, empty, writer, tabNum);
                }
                GenerateCSSummaryGetTypeMethod(datas, empty, writer, tabNum);
            }
        }

        public static void BeginGenerateCSSummaryClass(string name, StreamWriter writer, int tabNum)
        {
            if (writer != null)
            {
                string tab = TabString(tabNum);
                writer.WriteLine(tab + "public static partial class {0}", name);
                writer.WriteLine(tab + "{");
            }
        }
        public static void EndGenerateCSSummaryClass(StreamWriter writer, int tabNum)
        {
            string tab = TabString(tabNum);
            writer.WriteLine(tab + "}");
        }
        private static void GenerateTitles(StreamWriter writer)
        {
            writer.WriteLine(@"/*");
            writer.WriteLine(@"This file is Auto-Generated.Please do not try to alter any part.");
            writer.WriteLine(@"*/");

        }

        public static StreamWriter BeginGenerateCSSummaryNamespace(string path, string className, int tabNum)
        {
            string fileName = Path.Combine(path, className + ".cs");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            FileInfo file = new FileInfo(fileName);
            FileStream stream = file.OpenWrite();
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine("#pragma warning disable  0219");

            GenerateTitles(writer);
            GenerateDefination(writer, tabNum);
            writer.WriteLine("namespace StaticData");
            writer.WriteLine("{");

            return writer;
        }




        public static void EndGenerateCSSummaryNamespace(StreamWriter writer, int tabNum)
        {
            string tab = TabString(tabNum);
            writer.WriteLine(tab + "}");
        }
        private static void GenerateCSSummaryBinaryReadMethod(List<string> datas, bool empty, StreamWriter writer, int tabNum)
        {
            _GenerateCSSummaryStaticDataClassRelatedMethod(datas, empty, "Read", "void", "BinaryFileReader reader ,out IStaticData obj, string typeName", "typeName", (name, tab) =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    return tab + "obj = null;\n" + tab + "break;";
                }

                return tab + string.Format("{0} _{0};\n" + tab + "Read(reader,out _{0});\n" + tab + "obj = _{0};", name) + "\n" + tab + "break;";
            }, writer, tabNum);
        }

        private static void GenerateCSSummaryReadMethod(List<string> datas, bool empty, StreamWriter writer, int tabNum)
        {
            _GenerateCSSummaryStaticDataClassRelatedMethod(datas, empty, "Read", "void", "XmlNode node ,out IStaticData obj, string typeName", "typeName", (name, tab) =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    return tab + "obj = null;\n" + tab + "break;";
                }

                return tab + string.Format("{0} name{0};\n" + tab + "Read(node,out name{0});\n" + tab + "obj = name{0};", name) + "\n" + tab + "break;";
            }, writer, tabNum);
        }
        private static void GenerateCSSummaryBinaryWriteMethod(List<string> datas, bool empty, StreamWriter writer, int tabNum)
        {
            _GenerateCSSummaryStaticDataClassRelatedMethod(datas, empty, "Write", "void", "BinaryWriter writer, IStaticData obj", "obj.GetType().Name", (name, tab) =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    return tab + "break;";
                }

                return tab + string.Format("Write(writer, obj as {0});", name) + "\n" + tab + "break;";
            }, writer, tabNum);

        }

        private static void GenerateCSSummaryWriteMethod(List<string> datas, bool empty, StreamWriter writer, int tabNum)
        {
            _GenerateCSSummaryStaticDataClassRelatedMethod(datas, empty, "Write", "void", "XmlDocument doc, XmlNode father, IStaticData obj, string label = \"\"", "obj.GetType().Name", (name, tab) =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    return tab + "break;";
                }

                return tab + string.Format("Write(doc, father, obj as {0}, label);", name) + "\n" + tab + "break;";
            }, writer, tabNum);

        }
        private static void GenerateCSSummaryCreateMethod(List<string> datas, bool empty, StreamWriter writer, int tabNum)
        {
            _GenerateCSSummaryStaticDataClassRelatedMethod(datas, empty, "Create", "IStaticData", "System.Type type", "type.Name", (name, tab) =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    return tab + "return null;";
                }
                return string.Format(tab + "return new {0}();", name);
            }, writer, tabNum);
        }
        private static void GenerateCSSummaryStringSerializeMethod(List<string> datas, bool empty, StreamWriter writer, int tabNum)
        {
            _GenerateCSSummaryStaticDataClassRelatedMethod(datas, empty, "StringSerialize", "string", "IStaticData data", "data.GetType().Name", (name, tab) =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    return tab + "return \"\";";
                }
                return string.Format(tab + "return StringSerialize(data as {0});", name);
            }, writer, tabNum);
        }

        private static void GenerateCSSummaryGetTypeMethod(List<string> datas, bool empty, StreamWriter writer, int tabNum)
        {
            _GenerateCSSummaryStaticDataClassRelatedMethod(datas, empty, "GetType", "System.Type", "string type", "type", (name, tab) =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    return tab + "return null;";
                }
                return string.Format(tab + "return typeof({0});", name);
            }, writer, tabNum);
        }
        private static void _GenerateCSSummaryStaticDataClassRelatedMethod(List<string> datas, bool empty, string methodName, string methodReturn, string methodParams, string switchCondition, Func<string, string, string> caseBlock, StreamWriter writer, int tabNum)
        {
            if (writer != null)
            {
                string tab = StaticData.Generator.StaticDataUtility.TabString(tabNum);
                string tab2 = StaticData.Generator.StaticDataUtility.TabString(tabNum + 1);
                string tab3 = StaticData.Generator.StaticDataUtility.TabString(tabNum + 2);
                string tab4 = StaticData.Generator.StaticDataUtility.TabString(tabNum + 3);

                writer.WriteLine(tab + string.Format("public static {0} {1}({2})", methodReturn, methodName, methodParams));
                writer.WriteLine(tab + "{");
                writer.WriteLine(tab2 + string.Format("switch ({0})", switchCondition));
                writer.WriteLine(tab2 + "{");
                if (datas != null && !empty)
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        string config = datas[i];
                        
                        writer.WriteLine(tab3 + string.Format("case \"{0}\":", config));
                        writer.WriteLine(caseBlock(config, tab4));
                    }
                }

                writer.WriteLine(tab3 + "default:");
                writer.WriteLine(caseBlock("", tab4));
                writer.WriteLine(tab2 + "}");
                writer.WriteLine(tab + "}");
            }

        }



    }
}