using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Core.Extensions
{
    public static class DotNetExtensions
    {
        /// <summary>
        ///     二分查找小于等于target的index，可以指定在list中的一段中查找。默认返回的index在[l, r]中间。如果allowOutBound为true，
        ///     则当target小于所有值，则返回l - 1。
        /// </summary>
        /// <param name="list">排好序的List</param>
        /// <param name="target">目标</param>
        /// <param name="l">左index</param>
        /// <param name="r">右index</param>
        /// <param name="allowOutBound">允许返回超出界限的index。</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int FindLowerBound<T>(this IList<T> list, T target, int l = -1, int r = -1,
            bool allowOutBound = false) where T : IComparable<T>
        {
            if (l == -1) l = 0;
            if (r == -1) r = list.Count - 1;

            if (r < l)
            {
                Debug.Log("Error r index < l index");
                return -1;
            }

            var left  = l;
            var right = r;
            while (left < right)
            {
                var mid    = (right + left + 1) / 2;
                var result = list[mid].CompareTo(target);
                if (result == 0)
                    return mid;
                if (result > 0)
                    right = mid - 1;
                else
                    left = mid;
            }

            if (!allowOutBound) return left;

            {
                var result = target.CompareTo(list[left]);
                if (left == l && result < 0) return l - 1;
                return left;
            }
        }
    }

    public static class ListExtension
    {
        public static List<string> SortByStringCompare(this List<string> list, bool ascend = true)
        {
            var k = ascend ? 1 : -1;
            list.Sort((a, b) =>
            {
                return k * string.Compare(a, b);
            });
            return list;
        }

        public static List<T> RepeatedDefault<T>(int count)
        {
            return Repeated(default(T), count);
        }

        public static List<T> RepeatedEmpty<T>(int count, Func<int, T> createFunc)
        {
            var ret = new List<T>(count);
            for (var i = 0; i < count; i++)
                ret.Add(createFunc(i));
            return ret;
        }

        public static List<T> Repeated<T>(T value, int count)
        {
            var ret = new List<T>(count);
            ret.AddRange(Enumerable.Repeat(value, count));
            return ret;
        }

        public static void Shuffle<T>(this IList<T> list, Random rand = null)
        {
            rand = rand ?? new Random();
            var index  = 0;
            var length = list.Count;
            while (index < length - 1)
            {
                var r = rand.Next(index, length);
                var t = list[r];
                list[r]     = list[index];
                list[index] = t;
                index++;
            }
        }
    }

    public static class ArrayExtension
    {
        public static void Shuffle<T>(this T[] list, Random rand = null)
        {
            rand = rand ?? new Random();
            var index  = 0;
            var length = list.Length;
            while (index < length - 1)
            {
                var r = rand.Next(index, length);
                var t = list[r];
                list[r]     = list[index];
                list[index] = t;
                index++;
            }
        }

        public static int[] Range(int start, int end)
        {
            var ret = new int[end - start + 1];
            for (int i = 0; i < end - start + 1; i++)
            {
                ret[i] = start + i;
            }
            
            return ret;
        }

        public static T[] Copy<T>(T[] source)
        {
            var ret = new T[source.Length];
            Array.Copy(source, ret, source.Length);
            return ret;
        }

        public static HashSet<T> ToHashSet<T>(this T[] arr)
        {
            var ret = new HashSet<T>();
            foreach (var i in arr)
                ret.Add(i);
            return ret;
        }

        public static void RemoveAll<T>(this LinkedList<T> list, T element) where T : class
        {
            if (list.Count == 0) return;

            var cur = list.First;
            while (cur != null)
            {
                var next = cur.Next;
                if (cur.Value == element)
                {
                    list.Remove(cur);
                }
                
                cur = next;
            }
        }
    }
}