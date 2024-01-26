using EchoEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace StaticDataEditor
{
    public partial class ConcretDependency : BinaryDependencyInterface
    {
        //mingcai
        private string[] _XMLStaticDataPaths = new string[] { "xml" };

        public string[] GetXMLStaticDataPaths()
        {
            return _XMLStaticDataPaths;
        }
        public string GetBinaryStaticDataPath()
        {
            return StaticData.Facade.Instance.BinaryStaticDataPath;
        }


    }
}