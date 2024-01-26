using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Code.Core.Utils
{
    public static class CoreLog
    {
        public const string VERBOSE = "__LOG_VERBOSE";
        public const string WARNING = "__LOG_WARNING";
        // public const string ERROR = "__LOG_ERROR";

        //[Conditional(VERBOSE)]
        public static void Log(string s)
        {
            Debug.Log(s);
        }

        //[Conditional(VERBOSE)]
        public static void Log(object s)
        {
            Debug.Log(s);
        }

        public static void LogWarning(string s)
        {
            Debug.LogWarning(s);
        }

        public static void LogWarning(object s)
        {
            Debug.LogWarning(s);
        }

        // [Conditional(VERBOSE)]
        // [Conditional(WARNING)]
        // [Conditional(ERROR)]
        public static void LogError(string s)
        {
            Debug.LogError(s);
        }

        // [Conditional(VERBOSE)]
        // [Conditional(WARNING)]
        // [Conditional(ERROR)]
        public static void LogError(object s)
        {
            Debug.LogError(s);
        }
    }
}