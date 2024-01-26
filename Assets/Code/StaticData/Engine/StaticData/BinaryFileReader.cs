/************************************************
 created : 2018.08
 author : caiming
************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace StaticData
{
    public class BinaryFileReader
    {
        public BinaryFileReader()
        {
            _fullPath = _BinaryFileFullPath();
        }
        
        private void ReadVersion()
        {
            _version = _reader.ReadString();
            //Debug.Log("mingcai ReadVersion _version = " + _version);
        }
        public bool IsValid()
        {
            return _version == StaticDataSummary.Version && _version != string.Empty;
        }
        private void ReadTable()
        {
            int n = _reader.ReadInt32();

            for (int i = 0; i < n; i++)
            {
                int name = _reader.ReadInt32();
                string typeName = _reader.ReadString();
                _hash[typeName] = name;
            }
        }

        private void ReadTitle()
        {
            int n = _reader.ReadInt32();
            if (n > 0)
            {
                _titles = new BinaryFileTitle[n];
            }
            for (int i = 0; i < n; i++)
            {
                _titles[i] = new BinaryFileTitle();
                _titles[i].Category = _reader.ReadInt32();
                _titles[i].ID = _reader.ReadInt32();
                _titles[i].Length = _reader.ReadInt32();
            }
        }

        public bool BeginRead()
        {
            if(!File.Exists(_fullPath))
            {
                StaticDataUtility.LogWarning("Binary.raw 不存在 路径为 " + _fullPath);
                return false;
            }
            if(_reader == null)
            {
                System.IO.MemoryStream mStream = null;
                using (Stream stream = File.OpenRead(_fullPath))
                {
                    mStream = new MemoryStream();
                    stream.CopyTo(mStream);
                    stream.Close();
                }
                mStream.Seek(0, SeekOrigin.Begin);
                if (mStream == null)
                {
                    StaticDataUtility.LogWarning("Binary.raw 无法读取 路径为 " + _fullPath);
                    return false;
                }
                else
                {
                    StaticDataUtility.Log("Binary.raw 读取成功 路径为 " + _fullPath);
                }
                _reader = new System.IO.BinaryReader(mStream);
            }
            return Read();
        }

        private bool Read()
        {
            if(_dataPos == -1)
            {
                ReadVersion();
                if (!IsValid())
                {
                    _dataPos = -1;
                    StaticDataUtility.LogError("版本号不相同！");
                    return false;
                }
                ReadTable();
                ReadTitle();
                _dataPos = _reader.BaseStream.Position;
            }
            return true;
        }
        public bool ReadTemplate(string typeName, int id)
        {
            long pos = FindTemplateTitle(typeName, id);
            if(pos < 0)
            {
                //StaticDataUtility.LogWarning("读取Template 类型为 " + typeName + " id为 " + id.ToString() + " 的数据找不到");
                return false;
            }
            _reader.BaseStream.Seek(pos + _dataPos, System.IO.SeekOrigin.Begin);
            return true;
        }
        public bool ReadConfig(string typeName)
        {
            long pos = FindConfigTitle(typeName);
            if (pos < 0)
            {
                //StaticDataUtility.LogWarning("读取Config 类型为 " + typeName + " 的数据找不到");
                return false;
            }
            pos += _dataPos;
            _reader.BaseStream.Seek(pos, System.IO.SeekOrigin.Begin);
            return true;
        }


        public bool ReadBoolean()
        {
            return _reader.ReadBoolean();
        }
        public byte ReadByte()
        {
            return _reader.ReadByte();
        }
        public char ReadChar()
        {
            return _reader.ReadChar();
        }
        public decimal ReadDecimal()
        {
            return _reader.ReadDecimal();
        }
        public double ReadDouble()
        {
            return _reader.ReadDouble();
        }
        public short ReadInt16()
        {
            return _reader.ReadInt16();
        }
        public int ReadInt32()
        {
            int r = _reader.ReadInt32();
            return r;
        }
        public long ReadInt64()
        {
            return _reader.ReadInt64();
        }
        public sbyte ReadSByte()
        {
            return _reader.ReadSByte();
        }
        public float ReadSingle()
        {
            return _reader.ReadSingle();
        }
        public string ReadString()
        {
            return _reader.ReadString();
        }
        public ushort ReadUInt16()
        {
            return _reader.ReadUInt16();
        }
        public uint ReadUInt32()
        {
            return _reader.ReadUInt32();
        }
        public ulong ReadUInt64()
        {
            return _reader.ReadUInt64();
        }
        public Vector2 ReadVector2()
        {
            return new Vector2(_reader.ReadSingle(), _reader.ReadSingle());
        }
        public Vector3 ReadVector3()
        {
            return new Vector3(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle());
        }
        public Vector4 ReadVector4()
        {
            return new Vector4(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle());
        }
        public Color ReadColor()
        {
            return new Color(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle());
        }


        public void EndRead()
        {
            //if(_reader != null)
            //{
            //    _reader.Close();
            //    _reader = null;
            //}
            
        }

        private int FindConfigTitle(string typeName)
        {
            if(!_hash.ContainsKey(typeName))
            {
                //StaticDataUtility.LogWarning("Config 没有找到 类型为 " + typeName);
                return -1;
            }
            int sum = 0;
            
            int category = (int)_hash[typeName];
            for(int i = 0; i < _titles.Length; i ++)
            {
                if(_titles[i].Category == category)
                {
                    return sum;
                }
                sum += _titles[i].Length;
            }
            return -1;
        }
        public int[] FindTemplateIDs(string typeName)
        {
            if (!_hash.ContainsKey(typeName))
            {
                //StaticDataUtility.LogWarning("Config 没有找到 类型为 " + typeName);
                return null;
            }
            int category = (int)_hash[typeName];
            List<int> list = new List<int>();
            for (int i = 0; i < _titles.Length; i++)
            {
                if (_titles[i].Category == category)
                {
                    list.Add(_titles[i].ID);
                }
            }
            return list.ToArray();
        }
        public int[] FindTemplateTitles(string typeName)
        {
            if (!_hash.ContainsKey(typeName))
            {
                //StaticDataUtility.LogWarning("没找到!");
                return null;
            }
            int sum = 0;
            int category = (int)_hash[typeName];
            List<int> list = new List<int>();
            for (int i = 0; i < _titles.Length; i++)
            {
                if (_titles[i].Category == category)
                {
                    list.Add(_titles[i].ID);
                    list.Add(sum);
                }
                sum += _titles[i].Length;
            }
            return list.ToArray();
        }


        private int FindTemplateTitle(string typeName, int id)
        {
            if (!_hash.ContainsKey(typeName))
            {
                //StaticDataUtility.LogWarning("没找到!");
                return -1;
            }

            int sum = 0;
            int category = (int)_hash[typeName];
            for (int i = 0; i < _titles.Length; i++)
            {
                if (_titles[i].Category == category && _titles[i].ID == id)
                {
                    return sum;
                }
                sum += _titles[i].Length;
            }
            return -1;
        }


        public void Clear()
        {
            _hash.Clear();
            _titles = null;
            if (_reader != null)
            {
                _reader.Close();
                _reader = null;
            }
            _dataPos = -1;
            _fullPath = "";

        }
        private string _BinaryFileFullPath()
        {
            return Facade.Instance.BinaryFileFullPath;
            //return System.IO.Path.Combine(Constant.BinaryStaticDataPath, StaticData.StaticDataUtility.BelongBinaryFileName(null,1));
        }

        public string GetTypeName(int category)
        {
            IEnumerator enumerator = _hash.Keys.GetEnumerator();
            while(enumerator.MoveNext())
            {
                string key = (string)enumerator.Current;
                if((int)_hash[key] == category)
                {
                    return key;
                }
            }
            return string.Empty;
        }
        public int GetCategory(string name)
        {
            if(_hash.Contains(name))
            {
                return (int)_hash[name];
            }
            return -1;
        }

        private BinaryFileTitle[] _titles = null;
        private Hashtable _hash = new Hashtable();
        private System.IO.BinaryReader _reader = null;
        private long _dataPos = -1;
        private string _fullPath = "";
        private string _version = "";
    }

}