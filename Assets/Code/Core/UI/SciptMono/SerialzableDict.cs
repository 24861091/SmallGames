using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Serialization
{
    [Serializable]
    public class SerializableDict<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] [HideInInspector] private List<TKey> keyData = new List<TKey>();

        [SerializeField] [HideInInspector] private List<TValue> valueData = new List<TValue>();

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            for (var i = 0; i < keyData.Count && i < valueData.Count; i++) this[keyData[i]] = valueData[i];
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            keyData.Clear();
            valueData.Clear();

            foreach (var item in this)
            {
                keyData.Add(item.Key);
                valueData.Add(item.Value);
            }
        }
    }


    [Serializable]
    public class CompBindingDict : SerializableDict<string, Object>
    {
    }

    public enum PrimitiveType
    {
        @int,
        @float,

        @bool
        // @string, 
        // Vector3, 
    }

    [Serializable]
    public struct PrimitiveValue
    {
        public PrimitiveType Type;
        public string Value;
    }

    [Serializable]
    public class PrimitiveBindingDict : SerializableDict<string, PrimitiveValue>
    {
    }

    [Serializable]
    public class ListBindingDict : Dictionary<string, List<Object>>, ISerializationCallbackReceiver
    {
        [SerializeField] [HideInInspector] private List<string> keyData = new List<string>();

        [SerializeField] [HideInInspector] private List<int> lengthData = new List<int>();

        [SerializeField] [HideInInspector] private List<Object> valueData = new List<Object>();

        public void OnBeforeSerialize()
        {
            keyData.Clear();
            lengthData.Clear();
            valueData.Clear();
            foreach (var item in this)
            {
                var key = item.Key;
                var list = item.Value;
                keyData.Add(key);
                lengthData.Add(list.Count);
                valueData.AddRange(list);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            var index = 0;
            for (var keyIndex = 0; keyIndex < keyData.Count; keyIndex++)
            {
                if (!ContainsKey(keyData[keyIndex]))
                    Add(keyData[keyIndex], new List<Object>());
                var list = this[keyData[keyIndex]];
                var length = lengthData[keyIndex];
                for (var lengthIndex = index; lengthIndex < index + length; lengthIndex++)
                    list.Add(valueData[lengthIndex]);
            }
        }
    }
}