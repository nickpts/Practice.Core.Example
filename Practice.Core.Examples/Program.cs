using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Timers;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using Practice.Core.Examples.Comparers;
using Practice.Core.Examples.Concurrency;

namespace Practice.Core.Examples
{
    public delegate void PriceChangedHandler(decimal oldPrice, decimal newPrice);
    public delegate bool Predicate<T>(T someObject);
    public delegate void ProgressReporter(int percentComplete);

    public interface IMath
    {
        int Square(int x);
    }

    public class Squarer : IMath
    {
        public int Square(int x) => x * x;
    }

    public class Program
    {
        static void WriteProgressToConsole(int percentComplete) => Console.WriteLine(percentComplete);
        static void PlayAlertSound(int play) => Console.Beep();

        private static void Main(string[] args)
        {
            AdvancedThreading.ReleaseThreadsWithCountdownEvent();
            // AdvancedThreading.ReleaseMultipleThreadsWithWaitEventHandle();
            // AdvancedThreading.ThreadsWaitingOnAutoResetEvent();
            // AdvancedThreading.ReaderWriterLockSyncAccess();
            // AdvancedThreading.SemaphoreSixThreads();
            // AdvancedThreading.DeadlockExample();
            // AdvancedThreading.UseMutexToSynchronizeAccess();
            // BasicThreading.WaitExample();
            // BasicThreading.CPUandIOBoundOperationsTest();
            // BasicThreading.BasicTheadTest();
            // Concurrency.Asynchrony.TaskWithSimpleCancellation();
            // Asynchrony.TryWithTimeout();
            // Asynchrony.TaskCombinationsSyncInt();
            // Asynchrony.TaskCombinationsAsync();
            // Asynchrony.ReadFileAsyncCancellable();
            // Asynchrony.LoopExampleAsync();
            // Serialization.Test();
            // SomeNetworkingStuff.CookieWork();
            // Xml.Write();

            Thread.Sleep(100000);
        }
    }
}

#region delegates/events

//public class Util
//{
//    public static void DoWork(ProgressReporter monitor)
//    {
//        for (int i = 0; i < 10; i++)
//        {
//            monitor(i * 10);
//            System.Threading.Thread.Sleep(100);
//        }
//    }

//    public static void TransformAll(int[] values, IMath transformer)
//    {
//        for (int i = 0; i < values.Length; i++)
//        {
//            values[i] = transformer.Square(values[i]);
//        }
//    }
//}


#endregion

#region comparers





#endregion

public class Test
{
    public static void TestAction()
    {
        var dic = new SortedDictionary<string, string>(new SurnameComparer());
        dic.Add("MacPhail", "third!");
        dic.Add("MacBeth", "first!");
        dic.Add("Malcolm", "second!");

        foreach (string s in dic.Values) Console.WriteLine(s + " "); // first!, second!, third!

        string[] names = { "Tom", "Dick", "Harry", "Mary", "Jay" };


        string[] fullNames = { "Anne Williams", "John Fred Smith", "Sue Green" };
        IEnumerable<string> query = fullNames.SelectMany(name => name.Split()); // Anne | Williams | John | Fred | Smith | Sue | Green

        int[] numbers = { 3, 5, 7 };
        string[] words = { "three", "five", "seven", "ignored" };
        IEnumerable<string> zip = numbers.Zip(words, (n, w) => n + "=" + w);
        /* 3=three
         * 5=five
         * 7=seve
        */

        IEnumerable<string> oQuery = names.OrderBy(s => s.Length);
        // { "Jay", "Tom", "Mary", "Dick", "Harry" }

        IEnumerable<string> ooQuery = names.OrderBy(s => s.Length).ThenBy(s => s);
        // { "Jay", "Tom", "Dick", "Mary", "Harry" }

        string[] files = Directory.GetFiles("C:\\temp");
        var gQuery = files.GroupBy(f => Path.GetExtension(f));

        foreach (IGrouping<string, string> grouping in gQuery)
        {
            Console.WriteLine("Extension: " + grouping.Key);
            foreach (string fileName in grouping)
            {
                Console.WriteLine("   - " + fileName);
            }
        }

        /* Extension: .pdf
         * -- chapter03.pdf
         * -- chapter04.pdf
         * Extension: doc
         * -- todo.doc
         * -- menu.doc
        */

        int[] seq1 = { 1, 2, 3 };
        int[] seq2 = { 3, 4, 5 };

        IEnumerable<int> concat = seq1.Concat(seq2); // { 1, 2, 3, 3, 4, 5 }
        IEnumerable<int> union = seq1.Union(seq2); // { 1, 2, 3, 4, 5}

        XDocument fromWeb = XDocument.Load("http://albahari.com/sample.xml");
        XElement fromFile = XElement.Load(@"c:\media\somefile.xml");
        XElement config = XElement.Parse(@"<configuration><client enabled='true'><timeout>30</timeout></client></configuration>");

        foreach (XElement child in config.Elements()) Console.WriteLine(child.Name); // client
        XElement client = config.Element("client");

        bool enabled = (bool)client.Attribute("enabled"); // read attribute
        Console.WriteLine(enabled); // true

        client.Attribute("enabled").SetValue(!enabled); // update attribute
        int timeout = (int)client.Element("timeout"); // read element
        Console.WriteLine(timeout); // 30
        client.Element("timeout").SetValue(timeout * 2); // update element

        client.Add(new XElement("retries", 3)); // add new element
        Console.WriteLine(config); // implicitly call config.ToString();

        XElement customer = new XElement("customer", new XAttribute("id", 123),
            new XElement("firstname", "joe"),
            new XElement("lastname", "bloggs", new XComment("nice name")));

        var e1 = new XElement("test", "Hello");
        e1.Add("World");
        var e2 = new XElement("test", "Hello", "World");
        //both e1 and 2 end up with just one child XText element whose value is HelloWorld.
    }
}





