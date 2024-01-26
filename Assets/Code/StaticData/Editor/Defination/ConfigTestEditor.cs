using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StaticData
{
    [StaticData(ExportTarget.CSharp)]
    public class ConfigTestEditor : Config
    {
        public int a;
        public string b;
        public long c;
        public RuntimePlatform platform;
        public Vector3 v3;
    }

}
