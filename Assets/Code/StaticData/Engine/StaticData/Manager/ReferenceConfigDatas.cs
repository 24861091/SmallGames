using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaticData
{
    public class ReferenceConfigDatas : IReferenceConfigDatas
    {
        public bool Add(string key, Config val)
        {
            _configs.Add(key, val);
            return true;
        }

        public void Clear()
        {
            _configs.Clear();
        }

        public bool Contains(string key)
        {
            return _configs.ContainsKey(key);
        }

        public Config GetConfig(string key)
        {
            return _configs[key];
        }

        public bool Remove(string key)
        {
            return _configs.Remove(key);
        }
        private Dictionary<string, Config> _configs = new Dictionary<string, Config>();
    }
}