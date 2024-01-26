using System.Collections;
using System.Collections.Generic;
namespace StaticDataEditor
{
    public partial class FacadeEditor
    {
        private BinaryDependencyInterface binary = null;

        public void TryInitBinary()
        {
            if (binary == null)
            {
                binary = _interfaces.Get("BinaryDependencyInterfaceEditor") as BinaryDependencyInterface;
            }
        }

        public string[] XMLStaticDataPaths
        {
            get
            {
                return binary.GetXMLStaticDataPaths();
            }
        }
        public string BinaryStaticDataPath
        {
            get
            {
                return binary.GetBinaryStaticDataPath();
            }
        }



    }
}