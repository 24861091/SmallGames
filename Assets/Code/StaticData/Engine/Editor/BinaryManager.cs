using System.Collections;
using System.Collections.Generic;
using System.IO;
using StaticData;

namespace StaticDataEditor
{
    public class BinaryManager : CodeSingleton<BinaryManager>
    {
        public void SetVersion(string version)
        {
            _version = version;
        }
        public void Clear()
        {
            IEnumerator enumerator = _hash.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BinaryFile file = enumerator.Current as BinaryFile;
                if (file != null)
                {
                    file.Clear();
                }
            }
            _hash.Clear();
        }
        public void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
        public void Construct(IStaticData[] datas , string rootPath)
        {
            if (datas != null)
            {
                for (int i = 0; i < datas.Length; i++)
                {
                    Add(datas[i], rootPath);
                }
            }
        }

        private void Add(IStaticData data, string rootPath)
        {
            if (data != null)
            {
                string name = StaticDataUtility.BelongBinaryFileName(data, _maxBinaryFileNumbers);
                string fullpath = System.IO.Path.Combine(rootPath, name);
                if (!string.IsNullOrEmpty(name))
                {
                    BinaryFile file = null;
                    if (_hash.ContainsKey(name))
                    {
                        file = _hash[name] as BinaryFile;
                    }
                    else
                    {
                        file = new BinaryFile(fullpath, _version);
                        _hash.Add(name, file);
                    }
                    file.Add(data);
                }
            }
        }

        public void Save()
        {
            IEnumerator enumerator = _hash.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BinaryFile file = enumerator.Current as BinaryFile;
                if (file != null)
                {
                    file.Save();
                }
            }
        }
        public bool IsExisted()
        {
            IEnumerator enumerator = _hash.Values.GetEnumerator();
            bool exist = _hash.Count > 0;
            while (enumerator.MoveNext())
            {
                BinaryFile file = enumerator.Current as BinaryFile;
                if (file != null)
                {
                    exist = exist && file.Exist();
                }
            }
            return exist;
        }
        public void Copy(string from , string target)
        {
            Stack<string> stack = new Stack<string>();
            stack.Push(from);

            while (stack.Count > 0)
            {
                string path = stack.Pop();
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }
                string[] subs = Directory.GetDirectories(path);
                if (subs != null)
                {
                    for (int i = 0; i < subs.Length; i++)
                    {
                        stack.Push(subs[i]);
                    }
                }
                string[] files = Directory.GetFiles(path);
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        string file = files[i];
                        if (!string.IsNullOrEmpty(file))
                        {
                            string sourcePath = file;
                            string targetPath = file.Replace(from, target);
                            File.Copy(sourcePath, targetPath);
                        }
                    }
                }
            }

        }

        private Hashtable _hash = new Hashtable();
        private static int _maxBinaryFileNumbers = StaticData.Constant.MaxBinaryFileNumbers;
        private string _version = "";
    }

}