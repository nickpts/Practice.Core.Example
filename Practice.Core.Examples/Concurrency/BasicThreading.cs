using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using CSharp.Patterns.Concurrency;

namespace Practice.Core.Examples.Concurrency
{
    public class BasicThreading
    {
        public const string largeFileUri = @"C:\Users\Nick\Dev\docs\asyncioexample.txt";
        public const string shortFileUri = "";

        public static void BasicTheadTest()
        {
            List<string> longfileInput = ReadFile(largeFileUri);
            List<string> shortFileInput = new List<string>();

            Thread longActionThread = new Thread(() =>
            {
                longfileInput = ReadFile(@"C:\Users\Nick\Dev\docs\asyncioexample.txt");
            });

            Thread shortActionThread = new Thread(() => 
            {
                shortFileInput = ReadFile(@"C:\Users\Nick\Dev\docs\asyncioexample2.txt");
            });

            longActionThread.Start();
            shortActionThread.Start();

            longActionThread.Join();
            shortActionThread.Join();
            Console.WriteLine("jimmy!");

            int count = longfileInput.Count();
            int tCount = shortFileInput.Count();
            Console.WriteLine("johnny!");
        }

        public static void TestThreadParameters()
        {
            Thread t = new Thread(() => Print("Hello from t!"));

            string text = "t1";
            Thread t1 = new Thread(() => Console.WriteLine(text));

            text = "t2";
            Thread t2 = new Thread(() => Console.WriteLine(text));

            t1.Start();
            t2.Start();
            // t2 is printed twice!
        }

        public static void Print(string message)
        {
            Console.WriteLine(message);
        }

        public static List<string> ReadFile(string fileUri)
        {
            List<string> lines = new List<string>(); 

            using (StreamReader reader = File.OpenText(fileUri))
            {
                while (reader.Read() > -1)
                {
                    string line = reader.ReadLine();
                    lines.Add(line);
                }
            }

            return lines;
        }
    }
}
