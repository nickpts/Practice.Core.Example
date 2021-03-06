﻿using System;
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
        private static ManualResetEvent eve = new ManualResetEvent(false);

        private static Thread longCpuBoundThread = new Thread(IterateOverEnormousNumberSet);
        private static Thread shortCpuBoundThread = new Thread(IterateOverEnormousNumberSet);
        private static Thread longCpuBoundThreadWithHandle = new Thread(IterateOverEnormousNumberSetWithWait);

        /// <summary>
        /// Read two files from the disk at the same time
        /// </summary>
        public static void BasicTheadTest()
        {
	        List<string> longfileInput = new List<string>();
	        List<string> shortFileInput = new List<string>();

	        Thread longActionThread = new Thread(() =>
	        {
		        longfileInput = ReadFile(@"C:\Users\Nick\Dev\docs\asyncioexample.txt");
	        });

	        longActionThread.Name = "Long action thread";

	        Thread shortActionThread = new Thread(() =>
	        {
		        shortFileInput = ReadFile(@"C:\Users\Nick\Dev\docs\asyncioexample2.txt");
	        });

	        shortActionThread.Name = "Short action thread";

	        longActionThread.Start();
	        shortActionThread.Start();

	        // block the main thread
	        // by the time control is returned both threads will have returned
	        Thread.Sleep(10000);

	        int count = longfileInput.Count();
	        int tCount = shortFileInput.Count();
        }

        public void ThreadPoolStuff()
        {
            string testValue = string.Empty;
            ThreadPool.QueueUserWorkItem(new WaitCallback(a => GenericTestMethod("test")));
        }


        public static void DemonstrateManualResetEvent()
        {
            var signal = new ManualResetEvent(false);

            var t = new Thread(() => 
            {
                Console.WriteLine("Waiting for signal...");
                signal.WaitOne();
                signal.Dispose();
                Console.WriteLine("Signalled");
            });

            t.Start();
            Thread.Sleep(2000);

            signal.Set();
        }

        public static void WaitExample()
        {
            longCpuBoundThreadWithHandle.Start();

            //pressing enter should reset the thread.
            Console.ReadLine();
            eve.Set();
        }
        
        public static void CPUandIOBoundOperationsTest()
        {
            List<string> longfileInput = new List<string>();
            List<string> shortFileInput = new List<string>();

            shortCpuBoundThread.Name = "ShortCpuBoundThread";
            longCpuBoundThread.Name = "LongCpuBoundThread";

            Thread longIoBoundThread = new Thread(() =>
            {
                longfileInput = ReadFile(@"C:\Users\Nick\Dev\docs\asyncioexample.txt");
            });

            longIoBoundThread.Name = "LongIoBoundThread";

            Thread shortIoBoundThread = new Thread(() =>
            {
                shortFileInput = ReadFile(@"C:\Users\Nick\Dev\docs\asyncioexample2.txt");
            });

            shortIoBoundThread.Name = "ShortIoBoundThread";

            longCpuBoundThread.Start();
            shortCpuBoundThread.Start();
            longIoBoundThread.Start();
            shortIoBoundThread.Start();

            // block the main thread
            // switching to task manager should show activity across all cores
            Thread.Sleep(10000);

            int count = longfileInput.Count();
            int tCount = shortFileInput.Count();

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

        private static List<string> ReadFile(string fileUri)
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
        
        private static void IterateOverEnormousNumberSet()
        {
            for(double d = 0; d < 100000; d++)
            {
                // just loop over aimlessly
            }
        }

        private static void IterateOverReasonablySmallNumberSet()
        {
            for (int i = 0; i < 500000; i++)
            {
                // just loop over aimlessly
            }
        }

        private static void IterateOverEnormousNumberSetWithWait()
        {
            for (double d = 0; d < 100000; d++)
            {
                Console.WriteLine(d);

                if (d == 50000)
                {
                    eve.WaitOne();
                }
            }
        }

        private static void GenericTestMethod(string text)
        {
            // do nothing
        }
    }
}
