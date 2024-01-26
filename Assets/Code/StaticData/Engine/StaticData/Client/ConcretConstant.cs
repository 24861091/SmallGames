using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaticData
{
    public class ConcretDependency : Generator.GeneratorDependencyInterface
    {

        public void Log(string log)
        {
            //EchoEngine.Console.Log(log);
            Debug.Log(log);
        }

        public void LogDesinerError(string log)
        {
            //EchoEngine.Console.LogDesinerError(log);
            Debug.LogFormat("[Desiner]{0}",log);
        }

        public void LogError(string log)
        {
            //EchoEngine.Console.LogError(log);
            Debug.LogError(log);
        }

        public void LogWarning(string log)
        {
            //EchoEngine.Console.LogWarning(log);
            Debug.LogWarning(log);
        }
    }
}