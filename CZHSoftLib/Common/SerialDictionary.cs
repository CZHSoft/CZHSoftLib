using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CZHSoft.Common
{
    [Serializable]
    public class SerialDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        private string name;

        public SerialDictionary()
        {
            this.name = "SerialDictionary";
        }

        public SerialDictionary(string name)
        {
            this.name = name;
        }

        public void WriteXml(XmlWriter write)       // Serializer
        {
            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));


            write.WriteAttributeString("name", name);

            write.WriteStartElement(name);
            foreach (KeyValuePair<TKey, TValue> kv in this)
            {
                write.WriteStartElement("element");
                write.WriteStartElement("key");
                KeySerializer.Serialize(write, kv.Key);
                write.WriteEndElement();
                write.WriteStartElement("value");
                ValueSerializer.Serialize(write, kv.Value);
                write.WriteEndElement();
                write.WriteEndElement();
            }
            write.WriteEndElement();
        }
        
        public void ReadXml(XmlReader reader)       // Deserializer
        {
            reader.Read();
            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            name = reader.Name;
            reader.ReadStartElement(name);
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("element");
                reader.ReadStartElement("key");
                TKey tk = (TKey)KeySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue vl = (TValue)ValueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadEndElement();

                this.Add(tk, vl);
                reader.MoveToContent();
            }
            reader.ReadEndElement();
            reader.ReadEndElement();

        }
        
        public XmlSchema GetSchema()
        {
            return null;
        }
    }
}
