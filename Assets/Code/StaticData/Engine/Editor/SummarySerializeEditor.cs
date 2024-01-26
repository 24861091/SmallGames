using StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace StaticDataEditor
{
    public class SummarySerializeEditor : SummarySerializeInterface
    {
        public IStaticData Create(Type type)
        {
            return StaticDataSummaryEditor.Create(type);
        }

        public Type GetType(string type)
        {
            return StaticDataSummaryEditor.GetType(type);
        }

        public void Read(BinaryFileReader reader, out IStaticData obj, string typeName)
        {
            StaticDataSummaryEditor.Read(reader, out obj, typeName);
        }

        public void Read(XmlNode node, out IStaticData obj, string typeName)
        {
            StaticDataSummaryEditor.Read(node, out obj, typeName);
        }

        public void Write(BinaryWriter writer, IStaticData obj)
        {
            StaticDataSummaryEditor.Write(writer, obj);
        }

        public void Write(XmlDocument doc, XmlNode father, IStaticData obj, string label = "")
        {
            StaticDataSummaryEditor.Write(doc, father, obj, label);
        }
    }
}