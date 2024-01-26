using System.Collections;
using System.Collections.Generic;

namespace StaticDataEditor
{
    public static class Constant 
    {
        public static bool canGenerateEditorBinaryWrite = true;
        public static bool canGenerateEditorBinaryRead = true;
        public static bool canGenerateEditorXmlWrite = true;
        public static bool canGenerateEditorXmlRead = true;

        public static bool canGenerateRuntimeBinaryWrite = false;
        public static bool canGenerateRuntimeBinaryRead = true;
        public static bool canGenerateRuntimeXmlWrite = true;
        public static bool canGenerateRuntimeXmlRead = true;

        public static bool canGenerateLuaBinaryWrite = false;
        public static bool canGenerateLuaBinaryRead = true;
        public static bool canGenerateLuaXmlWrite = false;
        public static bool canGenerateLuaXmlRead = false;
    }
    public interface GeneratorDependencyInterface
    {
        string GetGenerateCSPath();
        string GetGenerateCSSummaryPath();
        string GetGenerateCSDefinationPath();
        string GetGenerateCSEditorPath();
        string GetGenerateCSEditorSummaryPath();
        string GetGenerateLuaPath();
    }
    public interface BinaryDependencyInterface
    {
        string[] GetXMLStaticDataPaths();
        string GetBinaryStaticDataPath();
    }
}