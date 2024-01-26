using EchoEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace StaticData
{
    public class BinaryConcretDependency : BinaryDependencyInterface
    {
        public string GetBinaryDebugStaticDataPath()
        {
            return Path.Combine(Application.temporaryCachePath, "DebugBinary");
        }
        public string GetBinaryStaticDataPath()
        {
            return "Assets/Resources/binary";
        }
        public string GetBinaryFileFullPath()
        {
            if (StaticDataUtility.UseBinary)
            {
                return Path.Combine(GetBinaryStaticDataPath(), StaticDataUtility.BelongBinaryFileName(null, 1));
                //string local = Path.Combine(ResourceConstants.BinaryDirectory, StaticDataUtility.BelongBinaryFileName(null, 1), ResourceMapping.MappingLocalPathSep);
                //return LoadResourceUrlUtils.GetFullPath(local);
            }
            else
            {
                return Path.Combine(GetBinaryDebugStaticDataPath(), StaticDataUtility.BelongBinaryFileName(null, 1));
                //return Path.Combine(Facade.Instance.BinaryDebugStaticDataPath, StaticDataUtility.BelongBinaryFileName(null, 1));
            }
        }
    }
}