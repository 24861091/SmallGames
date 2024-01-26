using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StaticData
{
    public class BinaryConfigLoader : IConfigLoader
    {
        public Config LoadConfig(string key, Func<Config, bool> Add, bool checkError = true)
        {
            Config c = null;
            IStaticData data;
            StaticData.StaticDataInterface.BeginRead(checkError);
            if(!StaticData.StaticDataInterface.ReadConfig(key))
            {
                return null;
            }
            StaticData.StaticDataInterface.Read(StaticData.StaticDataInterface.GetReader(), out data, key);
            Config config = data as Config;
            if (config != null)
            {
                if (!Add(config))
                {

                }
                else
                {
                    c = config;
                }
            }
            else if (checkError)
            {
                StaticDataUtility.LogError(string.Format("Config:{0} 在 binary.raw 中没有找到", key));
            }
            StaticData.StaticDataInterface.EndRead();
            return c;
        }
    }
}