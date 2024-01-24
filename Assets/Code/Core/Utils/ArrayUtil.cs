using System;

namespace Core.Utils
{
    public class ArrayUtil
    {
        public static void Clear(Array arr)
        {
            if (arr.GetLength(0) == arr.Length)
            {
                var length = arr.Length;
                for (int i = 0; i < length; i++)
                {
                    if (arr.GetValue(i) is Array itemArr)
                        Clear(itemArr);
                    else
                        arr.SetValue(null, i);
                }
            }
            else
            {
                Array.Clear(arr, 0, arr.Length);
            }
        }
        
        public static T[][] Create<T>(int row, int col)
        {
            var ret = new T[row][];
            for (int i = 0; i < row; i++)
                ret[i] = new T[col];
            return ret;
        }
    }
}