using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;
using StaticData;
using UnityEngine;
using UnityEditor;

namespace StaticDataEditor.Generator
{
    public static partial class StaticDataMenuCommand
    {
        private static List<string> _configs = new List<string>();
        private static List<string> _templates = new List<string>();
        private static List<string> _others = new List<string>();
        private static List<string> _editorConfigs = new List<string>();
        private static List<string> _editorTemplates = new List<string>();
        private static List<string> _editorOthers = new List<string>();

        private static List<Type> _binaryRead = new List<Type>();
        private static List<Type> _binaryWrite = new List<Type>();
        private static List<Type> _xmlWrite = new List<Type>();
        private static List<Type> _xmlRead = new List<Type>();


        private static List<string> _luas = new List<string>();
        private static List<string> _csharps = new List<string>();
        private static List<string> _all = new List<string>();



        //public static void Clear(Assembly assembly)
        //{
        //    clearCount = 0;
        //    ClearGenerated();
        //    GenerateConcret(assembly);
        //}
        public static void GenerateConcret(Assembly assembly)
        {
            Generate(false, assembly);
            //RefreshAfterGenerate();
        }
        public static void GenerateEmpty()
        {
            Generate(true, null);
        }
        public static void Generate(bool empty, Assembly targetAssembly)
        {
            StreamWriter csSummaryWriter = null;
            StreamWriter editorSummaryWriter = null;

            try
            {

                ClearList();


                csSummaryWriter = StaticDataEditor.SummaryClassGenerator.BeginGenerateCSSummaryNamespace(FacadeEditor.Instance.GenerateCSPath, "StaticDataSummary", 0);
                StaticDataEditor.SummaryClassGenerator.BeginGenerateCSSummaryClass(StaticData.Generator.Constant.SummaryClassName, csSummaryWriter, 1);

                editorSummaryWriter = StaticDataEditor.SummaryClassGenerator.BeginGenerateCSSummaryNamespace(FacadeEditor.Instance.GenerateCSEditorPath, "StaticDataSummaryEditor", 0);
                StaticDataEditor.SummaryClassGenerator.BeginGenerateCSSummaryClass(StaticData.Generator.Constant.SummaryEditorClassName, editorSummaryWriter, 1);
                if(!empty)
                {
                    Assembly assembly = targetAssembly; /*Assembly.GetAssembly(typeof(StaticData.StaticDataAttribute));*/
                    Type[] types = assembly.GetTypes();

                    List<string> errorTypes = new List<string>();
                    if (types != null)
                    {
                        for (int i = 0; i < types.Length; i++)
                        {
                            Type type = types[i];
                            StaticData.ExportTarget target = StaticDataEditor.Generator.StaticDataUtilityEditor.GetExportTarget(type);
                            if (target == StaticData.ExportTarget.None)
                            {
                                continue;
                            }
                            bool error = !_CheckOutputType(type);

                            if (!empty && error)
                            {
                                errorTypes.Add(type.Name);
                                StaticData.Generator.StaticDataUtility.LogErrorAndThrowException(string.Format("type = {0} 类型继承或类型不包含Editor,导出出错", type.Name));
                            }

                            if (!error)
                            {
                                _RecordClass(empty, type);
                                _GenerateSerializeDefination(type, target, empty);
                                _GenerateSerializeMethods(type, target, empty/*, csSummaryWriter, editorSummaryWriter*/);
                            }
                        }
                    }
                    ThrowException(errorTypes);
                }
                _GenerateToolMethods(empty, csSummaryWriter, editorSummaryWriter);
                
            }
            catch (Exception ex)
            {
                if (csSummaryWriter != null)
                {
                    csSummaryWriter.Close();
                }
                if (editorSummaryWriter != null)
                {
                    editorSummaryWriter.Close();
                }
                StaticData.Generator.StaticDataUtility.LogErrorAndThrowException(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                if (csSummaryWriter != null)
                {
                    csSummaryWriter.Close();
                }
                if (editorSummaryWriter != null)
                {
                    editorSummaryWriter.Close();
                }
            }
        }
        public static int clearCount = 0;
        public static void ClearPath(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
        }
        public static void ClearGenerated()
        {
            try
            {
                ClearPath(FacadeEditor.Instance.GenerateLuaPath);
                ClearPath(FacadeEditor.Instance.GenerateCSPath);

                clearCount++;
            }
            catch (Exception)
            {
                if (clearCount <= 3)
                {
                    ClearGenerated();
                }
            }
            finally
            {

            }

        }


        private static bool _CheckOutputType(Type type)
        {
            if (type.Name.IndexOf(StaticData.Generator.Constant.Suffix) < 0)
            {
                StaticData.Generator.StaticDataUtility.LogErrorAndThrowException(string.Format("Error: Type = {0} is not a Editor Class.", type.FullName));
                return false;
            }
            if (type.IsValueType)
            {
                StaticData.Generator.StaticDataUtility.LogErrorAndThrowException(string.Format("Error: Type = {0} is not a Editor Class.", type.FullName));
                return false;
            }
            if (type.Namespace != "StaticData")
            {
                StaticData.Generator.StaticDataUtility.LogErrorAndThrowException(string.Format("Error: Type = {0} is not a sub class of IStaticData.", type.FullName));
                return false;
            }

            Type father = type.BaseType;
            while (true)
            {
                if (father == typeof(object))
                {
                    StaticData.Generator.StaticDataUtility.LogErrorAndThrowException(string.Format("Error: Type = {0}'s Base Class is not a Editor Class.", type.FullName));
                    return false;
                }
                if (father == typeof(Template) || father == typeof(Config) || father == typeof(IStaticData))
                {
                    return true;
                }
                if (father.Name.IndexOf(StaticData.Generator.Constant.Suffix) < 0 && father.Namespace == "StaticData")
                {
                    StaticData.Generator.StaticDataUtility.LogErrorAndThrowException(string.Format("Error: Type = {0}'s Base Class is not a Editor Class.", type.FullName));
                    return false;
                }
                father = father.BaseType;
            }
        }
        private static void _RecordClass(bool empty, Type type)
        {
            if (empty)
            {
                return;
            }
            string generatedTypeName = StaticData.Generator.StaticDataUtility.GetGenerateClassName(type, StaticData.Generator.Constant.Suffix, false);
            if (type.IsSubclassOf(typeof(Config)))
            {
                if (!_configs.Contains(generatedTypeName))
                {
                    _configs.Add(generatedTypeName);
                }
                if (!_editorConfigs.Contains(type.Name))
                {
                    _editorConfigs.Add(type.Name);
                }
            }
            else if (type.IsSubclassOf(typeof(Template)))
            {
                if (!_templates.Contains(generatedTypeName))
                {
                    _templates.Add(generatedTypeName);
                }
                if (!_editorTemplates.Contains(type.Name))
                {
                    _editorTemplates.Add(type.Name);
                }
            }
            else
            {
                if (!_others.Contains(generatedTypeName))
                {
                    _others.Add(generatedTypeName);
                }
                if (!_editorOthers.Contains(type.Name))
                {
                    _editorOthers.Add(type.Name);
                }
            }
        }
        private static void _GenerateStructSerialize(string path, string className, bool isEditor, bool empty, int tab)
        {
            if (empty)
            {
                return;
            }
            StreamWriter writer = StaticDataEditor.SummaryClassGenerator.BeginGenerateCSSummaryNamespace(path, className + "_structs", 0);
            try
            {
                StaticDataEditor.SummaryClassGenerator.BeginGenerateCSSummaryClass(className, writer, 1);
                if (isEditor)
                {
                    if (Constant.canGenerateEditorBinaryRead)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateStructBinaryReadSerialize(_binaryRead, empty, writer, tab);
                    }
                    if (Constant.canGenerateEditorBinaryWrite)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateStructBinaryWriteSerialize(_binaryWrite, empty, writer, tab);
                    }
                    if (Constant.canGenerateEditorXmlRead)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateStructXmlReadSerialize(_xmlRead, empty, writer, tab);
                    }
                    if (Constant.canGenerateEditorXmlWrite)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateStructXmlWriteSerialize(_xmlWrite, empty, writer, tab);
                    }
                }
                else
                {
                    if (Constant.canGenerateRuntimeBinaryRead)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateStructBinaryReadSerialize(_binaryRead, empty, writer, tab);
                    }
                    if (Constant.canGenerateRuntimeBinaryWrite)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateStructBinaryWriteSerialize(_binaryWrite, empty, writer, tab);
                    }
                    if (Constant.canGenerateRuntimeXmlRead)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateStructXmlReadSerialize(_xmlRead, empty, writer, tab);
                    }
                    if (Constant.canGenerateRuntimeXmlWrite)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateStructXmlWriteSerialize(_xmlWrite, empty, writer, tab);
                    }
                }

                StaticDataEditor.SummaryClassGenerator.EndGenerateCSSummaryClass(writer, 1);
                StaticDataEditor.SummaryClassGenerator.EndGenerateCSSummaryNamespace(writer, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                writer.Close();
            }

        }
        private static void _GenerateToolMethods(bool empty, StreamWriter csSummaryWriter, StreamWriter editorSummaryWriter)
        {

            _GenerateStructSerialize(FacadeEditor.Instance.GenerateCSSummaryPath, StaticData.Generator.Constant.SummaryClassName, false, empty, 2);

            List<string> list = new List<string>();
            list.AddRange(_configs);
            list.AddRange(_templates);
            list.AddRange(_others);
            Except(list, _luas);
            
            StaticDataEditor.SummaryClassGenerator.GenerateVersion(csSummaryWriter, 2);
            StaticDataEditor.SummaryClassGenerator.GenerateCSSummaryClass(list, empty, csSummaryWriter, 2);
            StaticDataEditor.SummaryClassGenerator.EndGenerateCSSummaryClass(csSummaryWriter, 1);
            StaticDataEditor.SummaryClassGenerator.EndGenerateCSSummaryNamespace(csSummaryWriter, 0);

            _GenerateStructSerialize(FacadeEditor.Instance.GenerateCSEditorSummaryPath, StaticData.Generator.Constant.SummaryEditorClassName, true, empty, 2);

            list.Clear();
            list.AddRange(_editorConfigs);
            list.AddRange(_editorTemplates);
            list.AddRange(_editorOthers);
            StaticDataEditor.SummaryClassGenerator.GenerateCSSummaryEditorClass(list, empty, editorSummaryWriter, 2);
            StaticDataEditor.SummaryClassGenerator.EndGenerateCSSummaryClass(editorSummaryWriter, 1);
            StaticDataEditor.SummaryClassGenerator.EndGenerateCSSummaryNamespace(editorSummaryWriter, 0);

        }
        private static void GenerateCSFile(Type editorType)
        {
            StreamWriter writer = null;
            try
            {
                writer = GenerateFile(editorType, StaticData.Generator.Constant.Suffix, "cs", FacadeEditor.Instance.GenerateCSDefinationPath);
                StaticDataEditor.Generator.IStaticDataGenerator.GenerateCSFile(editorType, writer, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                writer.Close();
            }
        }
        private static void GenerateLUAFile(Type editorType)
        {
            StreamWriter writer = null;
            try
            {
                writer = GenerateFile(editorType, StaticData.Generator.Constant.Suffix, "lua", FacadeEditor.Instance.GenerateLuaPath);
                StaticDataEditor.LuaGenerator.GenerateLuaFile(editorType, writer, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                writer.Close();
            }
        }
        private static void _GenerateCSMethods(string typeFullName, string path, Type editorType, string className, bool empty, int tab)
        {
            StreamWriter writer = StaticDataEditor.SummaryClassGenerator.BeginGenerateCSSummaryNamespace(path, className + "_" + typeFullName.Replace("StaticData.", ""), 0);
            try
            {
                StaticDataEditor.SummaryClassGenerator.BeginGenerateCSSummaryClass(className, writer, 1);
                if (editorType.FullName == typeFullName)
                {
                    if (Constant.canGenerateEditorBinaryRead)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateCSBinaryReadMethod(typeFullName, editorType, _binaryRead, empty, writer, tab);
                    }
                    if (Constant.canGenerateEditorBinaryWrite)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateCSBinaryWriteMethod(typeFullName, editorType, _binaryWrite, empty, writer, tab);
                    }
                    if (Constant.canGenerateEditorXmlRead)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateCSReadMethod(typeFullName, editorType, _xmlRead, empty, writer, tab);
                    }
                    if (Constant.canGenerateEditorXmlWrite)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateCSWriteMethod(typeFullName, editorType, _xmlWrite, empty, writer, tab);
                    }
                }
                else
                {
                    if (Constant.canGenerateRuntimeXmlRead)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateCSReadMethod(typeFullName, editorType, _xmlRead, empty, writer, tab);
                    }
                    if (Constant.canGenerateRuntimeBinaryRead)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateCSBinaryReadMethod(typeFullName, editorType, _binaryRead, empty, writer, tab);
                    }
                    if (Constant.canGenerateRuntimeBinaryWrite)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateCSBinaryWriteMethod(typeFullName, editorType, _binaryWrite, empty, writer, tab);
                    }
                    if (Constant.canGenerateRuntimeXmlWrite)
                    {
                        StaticDataEditor.SummarySerializeGenerator.GenerateCSWriteMethod(typeFullName, editorType, _xmlWrite, empty, writer, tab);
                    }
                }
                StaticDataEditor.SummaryClassGenerator.EndGenerateCSSummaryClass(writer, 1);
                StaticDataEditor.SummaryClassGenerator.EndGenerateCSSummaryNamespace(writer, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                writer.Close();
            }

        }
        private static void _GenerateSerializeDefination(Type type, StaticData.ExportTarget target, bool empty)
        {
            Type editorType = type;
            switch (target)
            {
                case StaticData.ExportTarget.All:
                    GenerateCSFile(editorType);
                    if (!empty)
                    {
                        GenerateLUAFile(editorType);
                    }
                    break;
                case StaticData.ExportTarget.CSharp:
                    GenerateCSFile(editorType);
                    break;
                case StaticData.ExportTarget.Lua:
                    if (!empty)
                    {
                        GenerateLUAFile(editorType);
                    }
                    break;
                case StaticData.ExportTarget.None:
                    break;
            }
        }
        private static void _GenerateSerializeMethods(Type type, StaticData.ExportTarget target, bool empty/*, StreamWriter csSummaryWriter , StreamWriter editorSummaryWriter*/)
        {
            Type editorType = type;
            string runtimeTypeShortName = StaticData.Generator.StaticDataUtility.GetGenerateClassName(type, StaticData.Generator.Constant.Suffix, false);
            string runtimeTypeFullName = StaticData.Generator.StaticDataUtility.GetGenerateClassName(editorType, StaticData.Generator.Constant.Suffix);
            string editorTypeName = editorType.FullName;
            switch (target)
            {
                case StaticData.ExportTarget.All:
                    if (!empty)
                    {
                        _GenerateCSMethods(runtimeTypeFullName, FacadeEditor.Instance.GenerateCSSummaryPath, editorType, StaticData.Generator.Constant.SummaryClassName, empty, 2);
                        _GenerateCSMethods(editorTypeName, FacadeEditor.Instance.GenerateCSEditorSummaryPath, editorType, StaticData.Generator.Constant.SummaryEditorClassName, empty, 2);
                        _all.Add(runtimeTypeShortName);
                        GenerateLUAFile(editorType);
                    }
                    break;
                case StaticData.ExportTarget.CSharp:
                    if (!empty)
                    {
                        _GenerateCSMethods(runtimeTypeFullName, FacadeEditor.Instance.GenerateCSSummaryPath, editorType, StaticData.Generator.Constant.SummaryClassName, empty, 2);
                        _GenerateCSMethods(editorTypeName, FacadeEditor.Instance.GenerateCSEditorSummaryPath, editorType, StaticData.Generator.Constant.SummaryEditorClassName, empty, 2);
                        _csharps.Add(runtimeTypeShortName);
                    }
                    break;
                case StaticData.ExportTarget.Lua:
                    if (!empty)
                    {
                        _GenerateCSMethods(editorTypeName, FacadeEditor.Instance.GenerateCSEditorSummaryPath, editorType, StaticData.Generator.Constant.SummaryEditorClassName, empty, 2);
                        _luas.Add(runtimeTypeShortName);
                    }
                    break;
                case StaticData.ExportTarget.None:
                    break;
            }
        }

        private static void ClearList()
        {
            _configs.Clear();
            _templates.Clear();
            _others.Clear();
            _editorConfigs.Clear();
            _editorTemplates.Clear();
            _editorOthers.Clear();
            _binaryRead.Clear();
            _binaryWrite.Clear();
            _xmlWrite.Clear();
            _xmlRead.Clear();
            _luas.Clear();
            _csharps.Clear();
            _all.Clear();
        }

        private static void ThrowException(List<string> errorTypes)
        {
            if (errorTypes.Count > 0)
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                for (int i = 0; i < errorTypes.Count; i++)
                {
                    builder.Append(errorTypes[i]);
                    if (i != errorTypes.Count - 1)
                    {
                        builder.Append(",");
                    }
                }
                errorTypes.Clear();
                throw new Exception(string.Format("类型继承或类型不包含Editor,导出出错 types : " + builder.ToString()));
            }

        }


        private static void GenerateCSStaticNameArray(StreamWriter writer, int tabNum)
        {
            StaticDataEditor.SummaryClassGenerator.GenerateCSStaticNameArray("Configs", _configs, writer, tabNum);
            StaticDataEditor.SummaryClassGenerator.GenerateCSStaticNameArray("Templates", _templates, writer, tabNum);
            StaticDataEditor.SummaryClassGenerator.GenerateCSStaticNameArray("Others", _others, writer, tabNum);

            StaticDataEditor.SummaryClassGenerator.GenerateCSStaticNameArray("Luas", _luas, writer, tabNum);
            StaticDataEditor.SummaryClassGenerator.GenerateCSStaticNameArray("CSharps", _csharps, writer, tabNum);
            StaticDataEditor.SummaryClassGenerator.GenerateCSStaticNameArray("All", _all, writer, tabNum);

        }
        private static void GenerateCSStaticNameArrayEditor(StreamWriter writer, int tabNum)
        {
            StaticDataEditor.SummaryClassGenerator.GenerateCSStaticNameArray("Configs", _editorConfigs, writer, tabNum);
            StaticDataEditor.SummaryClassGenerator.GenerateCSStaticNameArray("Templates", _editorTemplates, writer, tabNum);
            StaticDataEditor.SummaryClassGenerator.GenerateCSStaticNameArray("Others", _editorOthers, writer, tabNum);

            StaticDataEditor.SummaryClassGenerator.GenerateCSStaticNameArray("Luas", _luas, writer, tabNum);
            StaticDataEditor.SummaryClassGenerator.GenerateCSStaticNameArray("CSharps", _csharps, writer, tabNum);
            StaticDataEditor.SummaryClassGenerator.GenerateCSStaticNameArray("All", _all, writer, tabNum);

        }

        private static void Except(List<string> list, List<string> except)
        {
            if (list == null || except == null)
            {
                return;
            }
            for (int i = 0; i < except.Count; i++)
            {
                list.Remove(except[i]);
            }
        }
        private static StreamWriter GenerateFile(Type editorType, string suffix, string ext, string generatePath)
        {
            if (!Directory.Exists(generatePath))
            {
                Directory.CreateDirectory(generatePath);
            }
            string path = Path.Combine(generatePath, editorType.Name.Substring(0, editorType.Name.Length - suffix.Length) + "." + ext);

            if (editorType == null)
            {
                return null;
            }
            if (!editorType.IsSubclassOf(typeof(IStaticData)))
            {
                return null;
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }


            FileInfo file = new FileInfo(path);
            FileStream stream = file.OpenWrite();
            StreamWriter writer = new StreamWriter(stream);
            return writer;
        }


        private static string TabString(int num)
        {
            return StaticData.Generator.StaticDataUtility.TabString(num);
        }
        private static string GetClassName(Type editorTypeDefinationClass)
        {
            return StaticData.Generator.StaticDataUtility.GetGenerateClassName(editorTypeDefinationClass, StaticData.Generator.Constant.Suffix);
        }


        [MenuItem("StaticData/Code Generate/Clear      (Engine Build)")]
        public static void GenerateEmptyTest()
        {
            StaticDataEditor.Generator.StaticDataMenuCommand.clearCount = 0;
            StaticDataEditor.Generator.StaticDataMenuCommand.ClearPath(StaticDataEditor.FacadeEditor.Instance.GenerateCSSummaryPath);
            StaticDataEditor.Generator.StaticDataMenuCommand.ClearPath(StaticDataEditor.FacadeEditor.Instance.GenerateCSEditorSummaryPath);
            StaticDataEditor.Generator.StaticDataMenuCommand.GenerateEmpty();
            AssetDatabase.ImportAsset(FacadeEditor.Instance.GenerateLuaPath);
            AssetDatabase.ImportAsset(FacadeEditor.Instance.GenerateCSPath);
            AssetDatabase.Refresh();
        }

    }
}