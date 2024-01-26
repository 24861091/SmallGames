using EchoEngine;
using StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StaticDataEditor
{
    public static partial class StaticDataMenuCommand
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Startup()
        {
            if (!StaticData.StaticDataUtility.UseBinary)
            {
                long begin = DateTime.Now.Ticks;
                CreateOriginBinary(StaticData.Constant.TempBinaryPath, StaticData.Facade.Instance.BinaryDebugStaticDataPath);
                _ShowStatusBar();

                long end = DateTime.Now.Ticks;
                TimeSpan span = new TimeSpan(end - begin);
                Debug.LogFormat("++++++++++++++++++++++++++++++++++" + span.TotalMilliseconds);

            }
        }
        static StaticDataMenuCommand()
        {
            EditorApplication.delayCall += () =>
            {
                Menu.SetChecked("StaticData/Binary/Use Binary", StaticDataUtility.UseBinary);
            };

            Register();
        }
        private static StaticDataEditor.ConcretDependency concret = new ConcretDependency();
        public static void Register()
        {
            Facade.Instance.Register("SummarySerializeInterface", new StaticData.SummarySerialize());
            Facade.Instance.Register("BinaryDependencyInterface", new StaticData.BinaryConcretDependency());
            Facade.Instance.Register("GeneratorDependencyInterface", new StaticData.ConcretDependency());

            Facade.Instance.TryInitBinary();
            Facade.Instance.TryInitSummarySerialize();
            Facade.Instance.TryInitGenerator();

            FacadeEditor.Instance.Register("SummarySerializeInterfaceEditor", new SummarySerializeEditor());
            if(concret == null)
            {
                concret = new ConcretDependency();
            }
            FacadeEditor.Instance.Register("BinaryDependencyInterfaceEditor", concret);
            FacadeEditor.Instance.Register("GeneratorDependencyInterfaceEditor", concret);

            FacadeEditor.Instance.TryInitBinary();
            FacadeEditor.Instance.TryInitSummarySerialize();
            FacadeEditor.Instance.TryInitGenerator();

        }

        //public static void ExecuteBuildBinary()
        //{
        //    Communication communication = new Communication();
        //    var echoResource = communication.GetString(Communication.ECHO_RES);
        //    ApplicationPathTool.EditorResourceRoot = echoResource;
        //    bool success = CreateOriginBinary(StaticData.Constant.TempBinaryPath, PathTool.Combine(ApplicationPathTool.EditorResourceRoot, "general", "binary"));
        //    EchoEngine.ApplicationPathTool.SetCSharpBuildTagFile(success);
        //}
        private static void RefreshAfterGenerate()
        {
            AssetDatabase.ImportAsset(FacadeEditor.Instance.GenerateLuaPath);
            AssetDatabase.ImportAsset(FacadeEditor.Instance.GenerateCSPath);
            AssetDatabase.Refresh();
        }
        public static void Clear()
        {
        }
        public static void GenerateConcret()
        {
            Generate(false);
        }
        public static void Generate(bool empty)
        {
            Generate(empty, Assembly.GetAssembly(typeof(StaticData.ConfigTestEditor)));
        }

        public static void GenerateConcret(Assembly assembly)
        {
            StaticDataEditor.Generator.StaticDataMenuCommand.GenerateConcret(assembly);
        }
        public static void Generate(bool empty, Assembly targetAssembly)
        {
            StaticDataEditor.Generator.StaticDataMenuCommand.Generate(empty, targetAssembly);
        }
        public static int clearCount
        {
            get
            {
                return StaticDataEditor.Generator.StaticDataMenuCommand.clearCount;
            }
            set
            {
                StaticDataEditor.Generator.StaticDataMenuCommand.clearCount = value;
            }
        }
        public static void ClearPath(string path)
        {
            StaticDataEditor.Generator.StaticDataMenuCommand.ClearPath(path);
        }
        public static void ClearGenerated()
        {
            StaticDataEditor.Generator.StaticDataMenuCommand.ClearGenerated();
        }

    }
}