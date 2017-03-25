using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Practice.Core.Examples.Abstractions
{
    public class Supplier
    {
        public const string XmlName = "supplier";
        public string name;

        public Supplier() { }
        public Supplier(XmlReader r)
        {
            ReadXml(r);
        }

        public void ReadXml(XmlReader r)
        {
            r.ReadStartElement();
            name = r.ReadElementContentAsString("name", string.Empty);
            r.ReadEndElement();
        }

        public void WriterXml(XmlWriter w)
        {
            w.WriteElementString("name", string.Empty);

        }
    }
}
