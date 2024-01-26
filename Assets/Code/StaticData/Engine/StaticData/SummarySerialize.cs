using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace StaticData
{
    public class SummarySerialize : SummarySerializeInterface
    {
        public IStaticData Create(Type type)
        {
            return StaticDataSummary.Create(type);
        }

        public Type GetType(string type)
        {
            return StaticDataSummary.GetType(type);
        }

        public void Read(BinaryFileReader reader, out IStaticData obj, string typeName)
        {
            StaticDataSummary.Read(reader, out obj, typeName);
        }

        public void Read(XmlNode node, out IStaticData obj, string typeName)
        {
            StaticDataSummary.Read(node, out obj, typeName);
        }

        public void Write(BinaryWriter writer, IStaticData obj)
        {
            //StaticDataSummary.Write(writer, obj);
        }

        public void Write(XmlDocument doc, XmlNode father, IStaticData obj, string label = "")
        {
            StaticDataSummary.Write(doc, father, obj, label);
        }
    }
}