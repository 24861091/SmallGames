namespace StaticData
{
    public partial class Facade 
    {

        #region constant
        private BinaryDependencyInterface _binary = null;

        public void TryInitBinary()
        {
            if(_binary == null)
            {
                _binary = _interfaces.Get("BinaryDependencyInterface") as BinaryDependencyInterface;
            }
        }
        public string BinaryFileFullPath
        {
            get
            {
                return _binary.GetBinaryFileFullPath();
            }
        }
        public string BinaryDebugStaticDataPath
        {
            get
            {
                return _binary.GetBinaryDebugStaticDataPath();
            }
        }
        public string BinaryStaticDataPath
        {
            get
            {
                return _binary.GetBinaryStaticDataPath();
            }
        }


        #endregion

    }
}