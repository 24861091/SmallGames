using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StaticData
{
    public class BinaryTemplateLoader : ITemplateLoader
    {
        public Template LoadTemplate(string key, int id, Func<Template, bool> Add, bool checkError = true)
        {
            Template temp = null;
            StaticData.StaticDataInterface.BeginRead(checkError);
            if (!StaticData.StaticDataInterface.ReadTemplate(key, id))
            {
                return null;
            }
            IStaticData data = null;
            StaticData.StaticDataInterface.Read(StaticData.StaticDataInterface.GetReader(), out data, key);
            Template template = data as Template;
            if (template != null)
            {
                if (Add(template))
                {
                    temp = template;
                }
            }
            else
            {
                if (checkError)
                {
                    StaticDataUtility.LogError("没有找到,是不是配错了");
                }
            }
            StaticData.StaticDataInterface.EndRead();
            return temp;
        }

        public Template[] LoadTemplates(string key, Func<Template, bool> Add, bool checkError = true)
        {
            Template[] temp = null;
            try
            {
                StaticData.StaticDataInterface.BeginRead(checkError);
                int[] ids = StaticData.StaticDataInterface.FindTemplateIDs(key);
                if (ids != null)
                {
                    for(int i = 0; i < ids.Length; i ++)
                    {
                        LoadTemplate(key, ids[i], Add, checkError);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                StaticData.StaticDataInterface.EndRead();
            }
            return temp;
        }
    }
}