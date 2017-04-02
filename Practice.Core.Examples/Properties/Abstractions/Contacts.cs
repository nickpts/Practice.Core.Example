using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Practice.Core.Examples.Abstractions
{
    public class Contacts
    {
        public IList<XCustomer> Customers = new List<XCustomer>();
        public IList<Supplier> Suppliers = new List<Supplier>();

        public void ReadXml(XmlReader r)
        {
            bool isEmpty = r.IsEmptyElement;
            r.ReadStartElement();
            if (isEmpty) return;
            while (r.NodeType == XmlNodeType.Element)
            {
                if (r.Name == XCustomer.XmlName) Customers.Add(new XCustomer(r));
                else if (r.Name == Supplier.XmlName) Suppliers.Add(new Supplier(r));
                else
                    throw new XmlException("Unexpected node " + r.Name);
            }

            r.ReadEndElement();
        }

        public void WriteXml(XmlWriter w)
        {
            foreach (XCustomer c in Customers)
            {
                w.WriteStartElement(XCustomer.XmlName);
                c.WriteXml(w);
                w.WriteEndElement();
            }

            foreach (Supplier s in Suppliers)
            {
                w.WriteStartElement(Supplier.XmlName);
                s.WriterXml(w);
                w.WriteEndElement();
            }

            w.Close();
        }
    }
}
