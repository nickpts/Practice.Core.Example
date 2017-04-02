using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Practice.Core.Examples.Abstractions
{
    public class XCustomer
    {
        public string firstName, lastName;
        public int? ID;
        public const string XmlName = "customer";

        public XCustomer() { }
        public XCustomer(XmlReader r)
        {

        }

        public void ReadXml(XmlReader r)
        {
            if (r.MoveToAttribute("id")) ID = r.ReadContentAsInt();

            r.ReadStartElement();
            firstName = r.ReadElementContentAsString("firstname", string.Empty);
            lastName = r.ReadElementContentAsString("lastname", string.Empty);
            r.ReadEndElement();

        }

        public void WriteXml(XmlWriter w)
        {
            if (ID.HasValue) w.WriteAttributeString("id", string.Empty, ID.ToString());
            w.WriteElementString("firstname", firstName);
            w.WriteElementString("lastname", lastName);

        }
    }
}
