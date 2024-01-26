using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StaticData
{
#if RUNTIME_XML

    public class XmlConfigLoader : IConfigLoader
    {
        public Config LoadConfig(string key, Func<Config, bool> Add, bool checkError)
        {
            Config c = null;
            IStaticData data = null;
            try
            {
                StaticDataUtility.TranverseElementXML(key, (element, fileName) =>
                {
                    StaticDataSummary.Read(element, out data, key);
                    Config config = data as Config;
                    if (config != null)
                    {
                        if (!Add(config))
                        {
                            if (checkError)
                            {
                                StaticDataUtility.LogError(string.Format("Config:{0} 重复了. fileName = {1}", key, fileName));
                            }
                        }
                        else
                        {
                            c = config;
                        }
                        return true;
                    }
                    else
                    {
                        if (checkError)
                        {
                            StaticDataUtility.LogError(string.Format("Config:{0} 在XML中没有找到. fileName = {1}", key, fileName));
                        }
                        return false;
                    }

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                StaticData.StaticDataLuaInterface.EndReadXML();
            }
            return c;

        }
    }
#endif
}