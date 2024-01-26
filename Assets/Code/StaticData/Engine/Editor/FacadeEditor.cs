using StaticData;
using System;
using System.IO;
using System.Xml;


namespace StaticDataEditor
{
    public partial class FacadeEditor 
    {
        private StaticData.SummarySerializeInterface _serialize = /*new SummarySerializeEditor();*/null;
        private Interfaces _interfaces = new Interfaces();

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
                _serialize = _interfaces.Get("SummarySerializeInterfaceEditor") as SummarySerializeInterface;
            }
        }
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
    }
}