using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StaticData
{
    public class WeakReferenceTemplateDatas : IReferenceTemplateDatas
    {
        public bool Add(string key, int id, Template val, bool canCoverData)
        {
            Dictionary<int, WeakReference> hash = null;
            if (_templates.ContainsKey(key))
            {
                hash = _templates[key];
                if (hash == null)
                {
                    hash = new Dictionary<int, WeakReference>();
                    _templates[key] = hash;
                }
                if (!canCoverData && hash.ContainsKey(id) && hash[id].IsAlive)
                {
                    return false;
                }
            }
            else
            {
                hash = new Dictionary<int, WeakReference>();
                _templates.Add(key, hash);
            }

            if (!hash.ContainsKey(id))
            {
                hash[id] = new WeakReference(val, false);
            }
            else
            {
                hash[id].Target = val;
            }

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
            return Contains(key) && _templates[key] != null && _templates[key].ContainsKey(id) && _templates[key][id] != null && _templates[key][id].IsAlive;
        }

        public Template GetTemplate(string key, int id)
        {
            return _templates[key][id].Target as Template;
        }
        public bool Remove(string key)
        {
            return _templates.Remove(key);
        }
        public bool Remove(string key, int id)
        {
            if (_templates.ContainsKey(key))
            {
                Dictionary<int, WeakReference> hash = _templates[key];
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
            while (enumerator.MoveNext())
            {
                WeakReference wr = enumerator.Current as WeakReference;
                if( wr != null)
                {
                    ts[i++] = wr.Target as Template;
                }
            }
            return ts;
        }

        private Dictionary<string, Dictionary<int, WeakReference>> _templates = new Dictionary<string, Dictionary<int, WeakReference>>();

    }
}