using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaticData
{
    public class ReferenceTemplateDatas : IReferenceTemplateDatas
    {
        public bool Add(string key, int id, Template val, bool canCoverData)
        {
            Dictionary<int, Template> hash = null;
            if (_templates.ContainsKey(key))
            {
                hash = _templates[key];
                if (hash == null)
                {
                    hash = new Dictionary<int, Template>();
                    _templates[key] = hash;
                }
                else if (!canCoverData && hash.ContainsKey(id))
                {
                    return false;
                }
            }
            else
            {
                hash = new Dictionary<int, Template>();
                _templates[key] = hash;
            }

            hash[id] = val;
            return true;
        }

        public void Clear()
        {
            _templates.Clear();
        }
        public bool Contains(string key)
        {
            return _templates.ContainsKey(key);
        }

        public bool Contains(string key, int id)
        {
            return Contains(key) && _templates[key] != null && _templates[key].ContainsKey(id);
        }

        public Template GetTemplate(string key, int id)
        {
            return _templates[key][id];
        }
        public bool Remove(string key)
        {
            return _templates.Remove(key);
        }

        public bool Remove(string key, int id)
        {
            if (_templates.ContainsKey(key))
            {
                Dictionary<int, Template> hash = _templates[key];
                if (hash != null)
                {
                    hash.Remove(id);
                    return true;
                }
            }
            return false;
        }

        public Template[] GetTemplates(string key)
        {
            Template[] ts = new Template[_templates[key].Values.Count];
            IEnumerator enumerator = _templates[key].Values.GetEnumerator();
            int i = 0;
            while(enumerator.MoveNext())
            {
                ts[i++] = enumerator.Current as Template;
            }
            return ts;
        }

        private Dictionary<string, Dictionary<int, Template>> _templates = new Dictionary<string, Dictionary<int, Template>>();

    }
}