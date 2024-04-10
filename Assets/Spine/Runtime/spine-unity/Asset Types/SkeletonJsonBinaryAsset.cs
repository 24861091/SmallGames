using UnityEngine;

namespace Spine.Unity
{
    public class SkeletonJsonBinaryAsset : TextAsset
    {
        private byte[] _data;
        
        public SkeletonJsonBinaryAsset(byte[] data)
        {
            _data = data;
        }

        public new byte[] bytes => _data;
    }
}