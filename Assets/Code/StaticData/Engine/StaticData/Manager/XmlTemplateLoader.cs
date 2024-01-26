using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StaticData
{
#if RUNTIME_XML
    public class XmlTemplateLoader : ITemplateLoader
    {
        public Template LoadTemplate(string key, int id, Func<Template, bool> Add, bool checkError = true)
        {
            Template temp = null;
            try
            {

                StaticDataUtility.TranverseElementXML(key, (element, fileName) =>
                {
                    IStaticData data = null;
                    
                    StaticDataSummary.Read(element, out data, key);
                    Template template = data as Template;
                    if (template != null)
                    {
                        if (template.ID == id)
                        {
                            if (!Add(template))
                            {
                                StaticDataUtility.LogError(string.Format("Template:{0} 重复了. id = {1} file = {2}", key, id.ToString(), fileName));
                            }
                            else
                            {
                                temp = template;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        if (checkError)
                        {
                            StaticDataUtility.LogError(string.Format("Template:{0} id:{1} 在XML的路径里没有找到文件,文件名是不是没有包含{0}?. file = {2}", key, id.ToString(), fileName));
                        }
                    }
                    return false;
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                StaticData.StaticDataLuaInterface.EndRead();
            }
            return temp;
        }
    }
#endif
}