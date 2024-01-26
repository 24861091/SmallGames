/************************************************
 created : 2018.8
 author : caiming
************************************************/
#pragma warning disable 0219
using System;


namespace StaticData
{
    public class ConfigManager : CodeSingleton<ConfigManager>
    {
        public ConfigManager()
        {
        }
        private string _Key(Type type)
        {
            return type.Name;
        }
        public bool Has(string key)
        {
            return configs.Contains(key);
        }
        public bool Add(Config value)
        {
            if(value != null)
            {
                return _Add(_Key(value.GetType()), value);
            }
            return false;
        }
        private bool _Add(string key, Config value)
        {
            if(Has(key))
            {
                return false;
            }            
            return configs.Add(key, value);
        }
        public bool Remove(Type type)
        {
            return _Remove(_Key(type));
        }
        private bool _Remove(string key)
        {
            return configs.Remove(key);
        }

        public void Clear()
        {
            configs.Clear();
            _configs = null;
        }

        private Config _LoadConfig(Type type, bool checkError = true)
        {
            Config config = null;
            config = _binaryLoader.LoadConfig(_Key(type), Add, checkError);
            return config;
        }

        public Config GetConfig(string key, bool checkError = true)
        {
            return GetConfig(StaticData.Facade.Instance.GetType(key), checkError);
        }
        public Config GetConfig(Type type, bool checkError = true)
        {
            Config config = null;
            if (type == null)
            {
                StaticDataUtility.LogError("你要找的Config类型是空的.");
            }
            string key = _Key(type);
            if (!Has(key))
            {
                config = _LoadConfig(type, checkError);
            }
            //if (!Has(key) && checkError)
            //{
            //    StaticDataUtility.LogError("你要找的Config没有找到,是不是配错了? Config:" + key);
            //}
            if(!Has(key))
            {
                return null;
            }
            return configs.GetConfig(key);
        }

        private IReferenceConfigDatas configs
        {
            get
            {
                if(_configs == null)
                {
                    if (Constant.IsCSharpWeakReference)
                    {
                        _configs = new WeakReferenceConfigDatas();
                    }
                    else
                    {
                        _configs = new ReferenceConfigDatas();
                    }
                }
                return _configs;
            }
        }

        private IReferenceConfigDatas _configs = null;
        private IConfigLoader _binaryLoader = new BinaryConfigLoader();
    }
}