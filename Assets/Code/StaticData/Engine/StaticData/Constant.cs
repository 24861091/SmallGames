using EchoEngine;
using UnityEngine;


namespace StaticData
{
    public static partial class Constant
    {

//#if RUNTIME_XML
        //public static string[] XMLStaticDataPaths = new string[] { PathTool.Combine(ApplicationPathTool.EditorResourceRoot, "xml") };
//#endif
        //public static string BinaryStaticDataPath = PathTool.Combine(ApplicationPathTool.ExportResourceRoot, ResourceConstants.BinaryDirectory);
        public static int MaxBinaryFileNumbers = 1;
        //public static string BinaryDebugStaticDataPath =  PathTool.Combine(Application.temporaryCachePath, "DebugBinary");
        public static string TempBinaryPath = "_TempBinary";
        public static string UseXmlKey = "_UseXml";

        public static string Suffix
        {
            get
            {
                return StaticData.Generator.Constant.Suffix;
            }
        }
        public static string SummaryClassName
        {
            get
            {
                return StaticData.Generator.Constant.SummaryClassName;
            }
        }

        public static string SummaryEditorClassName
        {
            get
            {
                return StaticData.Generator.Constant.SummaryEditorClassName;
            }
        }

        //public static int StaticDataUpLimit
        //{
        //    get
        //    {
        //        return StaticData.Generator.Constant.StaticDataUpLimit;
        //    }
        //}


        public static bool IsCSharpWeakReference = true;
        public static bool IsLuaWeakReference = true;
    }
    //interface GeneratorDependencyInterface
    //{
    //    void Log(string log);
    //    void LogWarning(string log);
    //    void LogError(string log);
    //    void LogDesinerError(string log);
    //}
    interface BinaryDependencyInterface
    {
        string GetBinaryFileFullPath();
        string GetBinaryDebugStaticDataPath();
        string GetBinaryStaticDataPath();
    }
}