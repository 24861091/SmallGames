/************************************************
 created : 2018.08
 author : caiming
************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaticData
{
    public struct BinaryFileTitle
    {
        public static BinaryFileTitle None = new BinaryFileTitle();
        public int Category;
        public int ID;
        public int Length;

        public static bool operator == (BinaryFileTitle file1, BinaryFileTitle file2)
        {
            return file1.Category == file2.Category && file1.ID == file2.ID && file1.Length == file2.Length;
        }
        public static bool operator !=(BinaryFileTitle file1, BinaryFileTitle file2)
        {
            return file1.Category != file2.Category || file1.ID != file2.ID || file1.Length != file2.Length;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return this == (BinaryFileTitle)obj;
        }

    }

}