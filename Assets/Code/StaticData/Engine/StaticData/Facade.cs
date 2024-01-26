using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;


namespace StaticData
{
    public partial class Facade
    {
        private StaticData.SummarySerializeInterface _serialize = null;
        


        public void Register(string k, object v)
        {
            _interfaces.Register(k, v);
        }

        public object Get(string k)
        {
            return _interfaces.Get(k);
        }

        public void TryInitSummarySerialize()
        {
            if(_serialize == null)
            {
                _serialize = _interfaces.Get("SummarySerializeInterface") as SummarySerializeInterface;
            }
        }
        #region serialize
        public IStaticData Create(Type type)
        {
            return _serialize.Create(type);
        }

        public Type GetType(string type)
        {
            return _serialize.GetType(type);
        }

        public void Read(BinaryFileReader reader, out IStaticData obj, string typeName)
        {
            _serialize.Read(reader, out obj, typeName);
        }

        public void Read(XmlNode node, out IStaticData obj, string typeName)
        {
            _serialize.Read(node, out obj, typeName);
        }

        public void Write(BinaryWriter writer, IStaticData obj)
        {
            _serialize.Write(writer, obj);
        }

        public void Write(XmlDocument doc, XmlNode father, IStaticData obj, string label = "")
        {
            _serialize.Write(doc, father, obj, label);
        }
        #endregion
    }
}