/************************************************
 created : 2018.8
 author : caiming
************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using EchoEngine;
using UnityEngine;

namespace StaticData
{
    public static partial class StaticDataUtility
    {
        public static string BelongBinaryFileName(IStaticData data, int max)
        {
            return "binary.raw";
        }
        public static bool UseXml
        {
            get
            {
                return PlayerPrefs.GetInt(Constant.UseXmlKey, 0) != 1;
            }
        }
        public static bool UseBinary
        {
            get
            {
                return (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer) || !UseXml;
            }
        }

        #region Log
        public static void BeginLogRecord(string information)
        {
            Generator.StaticDataUtility.BeginLogRecord(information);
        }
        public static void EndLogRecord()
        {
            Generator.StaticDataUtility.EndLogRecord();
        }
        public static void Log(string log)
        {
            Generator.StaticDataUtility.Log(log);
        }

        public static void LogWarning(string log)
        {
            Generator.StaticDataUtility.LogWarning(log);
        }
        public static void LogErrorAndThrowException(string log)
        {
            Generator.StaticDataUtility.LogErrorAndThrowException(log);
        }

        public static void LogError(string log)
        {
            Generator.StaticDataUtility.LogError(log);
        }
        public static void LogDesinerError(string log)
        {
            Generator.StaticDataUtility.LogDesinerError(log);
        }

        public static string LogInstance(object o, string name = "")
        {
            return Generator.StaticDataUtility.LogInstance(o, name);
        }

        #endregion
        #region 生成代码会调用
        public static string GetGenerateClassName(Type editorTypeDefinationClass, string suffix, bool nameSpace = true)
        {
            return Generator.StaticDataUtility.GetGenerateClassName(editorTypeDefinationClass, suffix, nameSpace);
        }
        public static string GetGenerateClassName(string editorTypeDefinationClassName, string suffix)
        {
            return Generator.StaticDataUtility.GetGenerateClassName(editorTypeDefinationClassName, suffix);
        }
        public static string GetGenerateClassName(string editorTypeDefinationClassName)
        {
            return Generator.StaticDataUtility.GetGenerateClassName(editorTypeDefinationClassName);
        }
        #endregion

    }
}