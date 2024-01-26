using EchoEngine;
using System.Collections;
using System.Collections.Generic;


namespace StaticDataEditor
{
    public partial class ConcretDependency : GeneratorDependencyInterface
    {
        public static string rootPath = @"Assets\Code\StaticData\StaticData\Generate\";
        public static string rootLuaPath = @"Assets\Code\StaticData\LuaScripts\StaticData\Generate\";
        public static string rootEditorPath = @"Assets\Code\StaticData\StaticData\Generate\";

        private static string _generateCSPath = @"";
        private static string _generateCSEditorPath = @"Editor";
        private static string _generateLuaPath = @"";
        private static string _generateCSSummaryPath = @"Summary";
        private static string _generateCSDefinationPath = @"Defination";
        private static string _generateCSEditorSummaryPath = @"Editor\Summary";



        public string GetGenerateCSPath()
        {
            return rootPath + _generateCSPath;
        }
        public string GetGenerateCSSummaryPath()
        {
            return rootPath + _generateCSSummaryPath;
        }
        public string GetGenerateCSDefinationPath()
        {
            return rootPath + _generateCSDefinationPath;
        }

        public string GetGenerateCSEditorPath()
        {
            return rootEditorPath + _generateCSEditorPath;
        }
        public string GetGenerateCSEditorSummaryPath()
        {
            return rootEditorPath + _generateCSEditorSummaryPath;
        }

        public string GetGenerateLuaPath()
        {
            return rootLuaPath + _generateLuaPath;
        }



    }
}