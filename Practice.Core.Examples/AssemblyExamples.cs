using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Resources;
using System.Drawing;
using System.Globalization;
using System.Collections;
using System.Threading;

namespace Practice.Core.Examples
{
    public class AssemblyExamples
    {

        #region Methods

        public static void StaticMethods()
        {
            Console.WriteLine(Assembly.GetExecutingAssembly()); 
            Console.WriteLine(Assembly.GetCallingAssembly()); 
            Console.WriteLine(Assembly.GetEntryAssembly());
           
            Console.ReadLine();
        }

        #endregion

        public void Test()
        {
            Assembly a = typeof(Program).Assembly;

            
            Assembly entry = Assembly.GetEntryAssembly();

            using (Stream s = entry.GetManifestResourceStream("TestProject.data.xml"))
            using (XmlReader r = XmlReader.Create(s))
            {

            }

            byte[] data;
            using (Stream s = entry.GetManifestResourceStream("TestProject.banner.jpg"))
            {
                data = new BinaryReader(s).ReadBytes((int)s.Length);
            }

            ResourceManager manager = new ResourceManager("resourceSource", Assembly.GetExecutingAssembly());

            ResourceSet set = manager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry en in set)
            {
                Console.WriteLine(en.Key);
            }

            Console.WriteLine("stamp".Substring(2)); // static invocation
        }
    }
}