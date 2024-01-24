using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utils
{
    public static class MiscUtil
    {
        public static string GetFullExceptionInfo(Exception e)
        {
            var sb = new StringBuilder();
            sb.AppendLine(e.Message);
            sb.AppendLine(e.StackTrace);
            if (e.InnerException != null)
            {
                sb.AppendLine("Inner Exception:");
                sb.AppendLine(GetFullExceptionInfo(e.InnerException));
            }

            return sb.ToString();
        }
        
        public static void SetMatrix<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            dict[key] = value;
        }

        public static TValue GetMatrix<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            if (dict.TryGetValue(key, out var ret))
            {
                return ret;
            }

            return defaultValue;
        }
    }
}