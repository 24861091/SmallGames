using EchoEngine;
using System.IO;

namespace StaticData.Generator
{
    public static partial class Constant
    {
        public static string Suffix = "Editor";
        public static string SummaryClassName = "StaticDataSummary";
        public static string SummaryEditorClassName = "StaticDataSummaryEditor";
        //public static int StaticDataUpLimit = 1000;
    }
    public interface GeneratorDependencyInterface
    {
        void Log(string log);
        void LogWarning(string log);
        void LogError(string log);
        void LogDesinerError(string log);
    }
}