using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaticData
{
    public interface IReferenceConfigDatas
    {
        bool Contains(string key);
        bool Add(string key, Config val);
        bool Remove(string key);
        void Clear();
        Config GetConfig(string key);
    }
    public interface IReferenceTemplateDatas
    {
        bool Contains(string key , int id);
        bool Contains(string key);
        bool Add(string key,int id, Template val, bool canCoverData);
        bool Remove(string key, int id);
        bool Remove(string key);
        void Clear();
        Template GetTemplate(string key, int id);

        Template[] GetTemplates(string key);
    }

}