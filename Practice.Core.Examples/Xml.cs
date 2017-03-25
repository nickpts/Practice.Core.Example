using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Practice.Core.Examples.Abstractions;

namespace Practice.Core.Examples
{ 
    public class Xml
    {
        public static void Write()
        {
            Contacts x = new Contacts();
            x.Customers.Add(new XCustomer() { firstName = "pete", ID = 1, lastName = "smith" });
            x.Customers.Add(new XCustomer() { firstName = "jack", ID = 2, lastName = "jones" });
            x.Suppliers.Add(new Supplier() { name = "drinks" });
            x.Suppliers.Add(new Supplier() { name = "food" });

            var settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;
            x.WriteXml(XmlWriter.Create(@"C:\Users\Nick\Downloads\contacts.xml", settings));
        }

        public static void Test()
        {
            //var bench = new XElement("bench",
            //    new XElement("toolbox",
            //        new XElement("handtool", "Hammer"),
            //        new XElement("handtool", "Rasp")
            //    ),
            //    new XElement("toolbox",
            //        new XElement("handtool", "Saw"),
            //        new XElement("powerool", "Nailgun")
            //    ),
            //    new XComment("Be careful with the naigul")
            //    );

            //IEnumerable<string> tools = from tool in bench.Elements("toolbox").Elements("handtool")
            //                            select tool.Value.ToUpper();

            //HAMMER, RASP, SAW

            //string hammer = bench.Element("toolbox").Element("handtool").Value;
            //// hammer

            //List<string> handtools = bench.Descendants("handtool").Select(c => c.Value).ToList();
            ////hammer, rasp, saw

            //foreach (XElement e in bench.Elements()) Console.WriteLine(e.Name + "=" + e.Value);

            //XElement e = new XElement("now", DateTime.Now);
            //DateTime dt = (DateTime)e;
            // toolbox=HammerRasp
            // toolbox=SawNailgun
            //int[] values = new int[] { 1, 2, 3, 4 };
            //Util.TransformAll(values, new Squarer());

            //ProgressReporter pMonitor = PlayAlertSound;
            //ProgressReporter p = x => Console.WriteLine("test");
            //pMonitor += WriteProgressToConsole;
            //Util.DoWork(pMonitor);

            //Stock s = new Examples.Stock("huh?");
            //s.Price = 4; // will print 4 to console
            //Func<string, string, int> result = (s1, s2) => s1.Length + s2.Length;
            //int length = result("test", "test"); // length is 8
        }

        public static void OtherXmlApiTest()
        {
            //var settings = new XmlReaderSettings();
            //settings.IgnoreWhitespace = true;

            //using (XmlReader reader = XmlReader.Create("customer.xml", settings))
            //while (reader.Read())
            //    {
            //        Console.Write(new string('', reader.Depth * 2)); // write info
            //        Console.WriteLine(reader.NodeType);
            //    }

            ///* XmlDeclaration
            // * Element
            // *  Element
            // *      Text
            // *  EndElement
            // *  Element
            // *      Text
            // *  EndElement
            // * EndElement
            // */

            var settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(@"C:\foo.xml", settings))
            {
                writer.WriteStartElement("customer");
                writer.WriteElementString("firstname", "Jim");
                writer.WriteElementString("lastname", "Bo");
                writer.WriteEndElement();
            }

            /* <?xml version="1.0" encoding="utf-8" ?>
             * <customer>
             *      <firstname>Jim</firstname>
             *      <lastname>Bo</lastname>
             * </customer>
             */
        }
    }
}
