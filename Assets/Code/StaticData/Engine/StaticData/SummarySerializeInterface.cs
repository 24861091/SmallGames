using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace StaticData
{
    public interface SummarySerializeInterface
    {
        IStaticData Create(System.Type type);
        void Read(BinaryFileReader reader, out IStaticData obj, string typeName);
        void Write(BinaryWriter writer, IStaticData obj);
        void Read(XmlNode node, out IStaticData obj, string typeName);
        void Write(XmlDocument doc, XmlNode father, IStaticData obj, string label = "");
        System.Type GetType(string type);
    }
}