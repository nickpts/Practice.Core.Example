using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.IO.Compression;

namespace Practice.Core.Examples
{
    public class Io
    {
        async static void AsyncDemo()
        {
            using (Stream s = new FileStream("test.txt", FileMode.Create))
            {
                byte[] block = { 1, 2, 3, 4, 5 };
                await s.WriteAsync(block, 0, block.Length); // write asynchronously
                s.Position = 0;
                Console.WriteLine(await s.ReadAsync(block, 0, block.Length)); // 5
            }
        }

        public async void Read()
        {
            // assuming s is a stream
            // byte[] data = new BinaryReader(s).ReadBytes(1000);
            string baseFolder = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(baseFolder, "logo.jpg");
            Console.WriteLine(File.Exists(path));

            using (FileStream fs = File.Create("text.txt"))
            using (TextWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine("Line1");
                writer.WriteLine("Line2");
            }

            using (FileStream fs = File.OpenRead("test.txt"))
            using (TextReader reader = new StreamReader(fs))
            {
                Console.WriteLine(reader.ReadLine()); // Line1
                Console.WriteLine(reader.ReadLine()); // Line2
            }

            using (TextWriter writer = File.CreateText("data.txt"))
            {
                writer.WriteLine("123");
                writer.WriteLine("true");
            }

            using (TextReader reader = File.OpenText("data.txt"))
            {
                int myInt = int.Parse(reader.ReadLine());
                bool yes = bool.Parse(reader.ReadLine());
            }

            using (TextWriter writer = File.AppendText("text.txt"))
                writer.WriteLine("Line3");

            using (TextReader reader = File.OpenText("test.txt"))
            {
                while (reader.Peek() > -1)
                {
                    Console.WriteLine(reader.ReadLine()); // Line1
                                                          // Line2
                                                          // Line3
                }
            }

            XmlReader r = XmlReader.Create(new StringReader("random string"));

            using (Stream s = File.Create("compressed.bin"))
            using (Stream ds = new DeflateStream(s, CompressionMode.Compress))
            {
                for (byte i = 0; i < 100; i++)
                {
                    ds.WriteByte(i);
                }
            }

            using (Stream s = File.OpenRead("compressed.bin"))
            using (Stream ds = new DeflateStream(s, CompressionMode.Decompress))
            {
                for (byte i = 0; i < 100; i++) Console.WriteLine(ds.ReadByte()); // writes 0 to 99
            }

            string localAppDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyCoolApplication");

            if (!Directory.Exists(localAppDataPath)) Directory.CreateDirectory(localAppDataPath);
        }

        public void ReadFile()
        {
            using (TextReader reader = new StreamReader(@"C:\varioustext.doc"))
            {
                
            }
        }

    }
}
