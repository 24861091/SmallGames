using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StaticData;

namespace StaticDataEditor
{
    public class BinaryFileComparer
    {
        public BinaryFileComparer(BinaryFile file1, BinaryFile file2)
        {
            _file1 = file1;
            _file2 = file2;
        }

        private bool IsValid()
        {
            return _file1 != null && _file2 != null;
        }

        private BinaryFileCompareResult Analysis()
        {
            BinaryFileCompareResult result = new BinaryFileCompareResult();
            if (!IsValid())
            {
                return result;
            }

            CompareHash(result);
            CompareTitle(result);   
            CompareData(result);

            return result;
        }
        
        private void CompareHash(BinaryFileCompareResult result)
        {
            Hashtable hash1 = _file1.GetHash();
            Hashtable hash2 = _file2.GetHash();

            IEnumerator enemerable1 = hash1.Keys.GetEnumerator();
            IEnumerator enemerable2 = hash2.Keys.GetEnumerator();
            while (enemerable1.MoveNext())
            {
                string key1 = enemerable1.Current.ToString();
                bool contains = false;
                while (enemerable2.MoveNext())
                {
                    string key2 = enemerable1.Current.ToString();
                    if (key1 == key2)
                    {
                        contains = true;
                        break;
                    }
                }
                if (contains)
                {
                    result.SharedDataNames.Add(key1);
                    result.SoloDataNames2.Remove(key1);
                }
                else
                {
                    result.SoloDataNames1.Add(key1);
                    
                }
            }
        }
        private void CompareTitle(BinaryFileCompareResult result)
        {

        }

        private void CompareData(BinaryFileCompareResult result)
        {

        }



        private BinaryFile _file1;
        private BinaryFile _file2;
    }


    public class BinaryFileCompareResult
    {
        public List<string> SharedDataNames = new List<string>();
        public List<string> SoloDataNames1 = new List<string>();
        public List<string> SoloDataNames2 = new List<string>();

        public List<BinaryFileTitle> SharedDataTitles = new List<BinaryFileTitle>();
        public List<BinaryFileTitle> SoloDataTitles1 = new List<BinaryFileTitle>();
        public List<BinaryFileTitle> SoloDataTitles2 = new List<BinaryFileTitle>();

        public List<BinaryFileTitle> SharedDatas = new List<BinaryFileTitle>();
        public List<BinaryFileTitle> SoloDatas1 = new List<BinaryFileTitle>();
        public List<BinaryFileTitle> SoloDatas2 = new List<BinaryFileTitle>();


    }

}