using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StaticData
{
    public class WeakReferenceConfigDatas : IReferenceConfigDatas
    {
        public bool Add(string key, Config val)
        {
            if (_configs.ContainsKey(key))
            {
                _configs[key].Target = val;
            }
            else
            {
                _configs.Add(key, new WeakReference(val, false));
            }
            return true;
        }

        public void Clear()
        {
            _configs.Clear();
        }

        public bool Contains(string key)
        {
            if (!_configs.ContainsKey(key))
            {
                return false;
            }
            return _configs[key].IsAlive;
        }

        public Config GetConfig(string key)
        {
            return _configs[key].Target as Config;
        }

        public bool Remove(string key)
        {
            return _configs.Remove(key);
        }
        private Dictionary<string, WeakReference> _configs = new Dictionary<string, WeakReference>();
    }
}