using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using StaticData;

namespace StaticDataEditor
{
    public class BinaryFile
    {
        public BinaryFile(string fullName, string version)
        {
            _fullName = fullName;
            _version = version;
        }
        //public BinaryFile(IStaticData [] datas, string fullName)
        //{
        //    _fullName = fullName;
        //    if (datas != null)
        //    {
        //        for(int i = 0; i < datas.Length; i ++)
        //        {
        //            IStaticData data = datas[i];
        //            if(data != null)
        //            {
        //                Add(data);
        //            }
        //        }
        //    }
        //}


        public void Add(IStaticData data)
        {
            _list.Add(data);
            int category = AddHash(data);
            BinaryFileTitle title = ConstructTitle(data, category);
            string key = Key(category, title.ID);
            
            AddTitle(key, title);
        }
        private BinaryFileTitle ConstructTitle(IStaticData data, int category)
        {
            BinaryFileTitle title = new BinaryFileTitle();
            Template template = data as Template;
            title.Category = category;
            title.ID = (template == null) ? -1 : template.ID;
            title.Length = 0;

            return title;
        }
        private int AddHash(IStaticData data)
        {
            int category = -1;
            string typeName = data.GetType().Name;
            if (!_hash.ContainsKey(typeName))
            {
                category = _hash.Count + 1;
                _hash.Add(typeName, category);
            }
            else
            {
                category = (int)_hash[typeName];
            }
            return category;
        }

        private BinaryFileTitle GetTitle(string key)
        {
            for(int i = 0; i < _titles.Count; i ++)
            {
                if(_titles[i].Key == key)
                {
                    return _titles[i].Value;
                }
            }
            return BinaryFileTitle.None;
        }
        private void AddTitle(string key, BinaryFileTitle title)
        {
            _titles.Add(new KeyValuePair<string, BinaryFileTitle>(key, title));
            _titlesHash[key] = _titles.Count - 1;
        }
        private void SetLength(string key, int length)
        {
            BinaryFileTitle title;
            if (_titlesHash.ContainsKey(key))
            {
                title = _titles[_titlesHash[key]].Value;
            }
            else
            {
                title = new BinaryFileTitle();
                StaticDataUtility.LogError(" 不包含Key= " + key);
            }
            title.Length = length;
            _titles[_titlesHash[key]] = new KeyValuePair<string, BinaryFileTitle>(key, title);

        }
        
        private string Key(IStaticData data)
        {
            if(data == null)
            {
                return "";
            }
            string typeName = data.GetType().Name;
            if (!_hash.ContainsKey(typeName))
            {
                return "";
            }
            Template template = data as Template;
            int index = (template == null) ? -1 : template.ID;
            return Key((int)_hash[typeName], index);
        }
        private string Key(int name , int index)
        {
            return string.Format("{0}_{1}", name, index);
        }

        public void Save()
        {
            try
            {
                if (!string.IsNullOrEmpty(_fullName))
                {
                    if (File.Exists(_fullName))
                    {
                        File.Delete(_fullName);
                    }
                    string directory = Path.GetDirectoryName(_fullName);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    Stream stream = File.Create(_fullName);
                    _writer = new BinaryWriter(stream);
                    SerializeVersion();
                    SerializeHash();
                    int length = CalcuateDataTitleLength();
                    long pos = _writer.BaseStream.Position;
                    _writer.Seek(length, SeekOrigin.Current);
                    SerializeData();
                    _writer.Seek((int)pos, SeekOrigin.Begin);
                    SerializeTitle();
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                _writer.Close();
                _writer = null;
            }
        }
        private void SerializeVersion()
        {
            _writer.Write(_version);
            Debug.Log("mingcai SerializeVersion _version = " + _version.ToString());
        }
        private void SerializeHash()
        {
            IEnumerator enumerator = _hash.Keys.GetEnumerator();
            _writer.Write(_hash.Count);
            while(enumerator.MoveNext())
            {
                string typeName = enumerator.Current.ToString();
                
                int name = (int)_hash[typeName];
                typeName = typeName.Replace(StaticData.Constant.Suffix, "");
                _writer.Write(name);
                _writer.Write(typeName);
            }
        }
        private void SerializeTitle()
        {
            _writer.Write(_titles.Count);
            for(int i = 0; i < _titles.Count; i ++)
            {
                BinaryFileTitle title = (BinaryFileTitle)_titles[i].Value;
                _writer.Write(title.Category);
                _writer.Write(title.ID);
                _writer.Write(title.Length);
            }
        }
        private int CalcuateDataTitleLength()
        {
            int sum = 0;
            sum += sizeof(int);
            sum += _titles.Count * sizeof(int) * 3;
            return sum;
        }
        private void SerializeData()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                IStaticData data = _list[i];
                if (data != null)
                {
                    _position = _writer.BaseStream.Position;
                    StaticDataEditor.FacadeEditor.Instance.Write(_writer, data);
                    long length = _writer.BaseStream.Position - _position;
                    string key = Key(data);

                    SetLength(key, (int)length);
                }
            }
        }

        public Hashtable GetHash()
        {
            return _hash;
        }
        public List<KeyValuePair<string, BinaryFileTitle>> GetTitles()
        {
            return _titles;
        }
        public List<IStaticData> GetDatas()
        {
            return _list;
        }
        public bool Exist()
        {
            if (!string.IsNullOrEmpty(_fullName))
            {
                return File.Exists(_fullName);
            }
            return false;
        }
        public void Clear()
        {
            _titlesHash.Clear();
            _titles.Clear();
            _hash.Clear();
            _list.Clear();
        }
        private string _fullName;
        private string _version;
        private List<IStaticData> _list = new List<IStaticData>();
        private BinaryWriter _writer = null;
        private Hashtable _hash = new Hashtable();
        private List<KeyValuePair<string, BinaryFileTitle>> _titles = new List<KeyValuePair<string, BinaryFileTitle>>();
        private Dictionary<string, int> _titlesHash = new Dictionary<string, int>();
        private long _position = 0;
    }

    
}