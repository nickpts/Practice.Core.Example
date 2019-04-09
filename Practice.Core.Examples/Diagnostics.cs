using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Timers;

namespace Practice.Core.Examples
{
    public class Diagnostics
    {
        private System.Timers.Timer timer;

        private Diagnostics()
        {
            timer = new System.Timers.Timer() { Interval = 1000 };
            timer.Elapsed += timerElapsed;
            timer.Start();
        }

        void timerElapsed(object sender, ElapsedEventArgs e) { }

        public static void Test()
        {
            var sb = new StringBuilder("this is a test");
            var weak = new WeakReference(sb);
            Console.WriteLine(weak.Target); // this is a test
            GC.Collect();
            Console.WriteLine(weak.Target); // nothing
        }

        [Conditional("LoggingMode")]
        public static void LogStatus(string ms)
        {

        }

        public void TraceSequnce()
        {
            // clears default listener
            Trace.Listeners.Clear();
            // adds a writer that appends to the trace.text file
            Trace.Listeners.Add(new TextWriterTraceListener("trace.txt"));

            // obtain the console's output stream then add that
            TextWriter tw = Console.Out;
            Trace.Listeners.Add(new TextWriterTraceListener(tw));

            // set up a windows event log source, then create/add listener.
            // CreateEventSource requires admin privileges
            if (!EventLog.SourceExists("DemoApp"))
            {
                EventLog.CreateEventSource("DemoApp", "Application");
            }

            Trace.Listeners.Add(new EventLogTraceListener("DemoApp"));

            var listener = new TextWriterTraceListener(Console.Out);
            // will be applied when using the Trace methods
            listener.TraceOutputOptions = TraceOptions.DateTime | TraceOptions.Callstack;


        }

        /// <summary>
        /// Huh?
        /// </summary>
        /// <typeparam name="T">test</typeparam>
        /// <param name="list">test</param>
        /// <param name="item">test</param>
        /// <returns>tet</returns>
        public static bool AddIfNotPresent<T>(IList<T> list, T item)
        {
            Contract.Requires(list != null);        // precondition
            Contract.Requires(!list.IsReadOnly);    // precondition
            Contract.Ensures(list.Contains(item));  // postcondition

            if (list.Contains(item)) return false;
            list.Add(item);

            return true;
        }


        const string SourceName = "MyApp";

        public void WriteEvent()
        {
            // requires admin privileges, so this is typicall done
            // in application setup

            if (!EventLog.SourceExists(SourceName))
            {
                EventLog.CreateEventSource(SourceName, "Application");
            }

            EventLog.WriteEntry(SourceName, "Service started: using configuration file=...",
                EventLogEntryType.Information);
        }

        public void ReadEvent()
        {
            EventLog log = new EventLog("Application");

            EventLogEntry last = log.Entries[log.Entries.Count - 1];
            Console.WriteLine("Index:   " + last.Index);
            Console.WriteLine("Source:   " + last.Source);
            Console.WriteLine("Type:     " + last.EntryType);
            Console.WriteLine("Time:     " + last.TimeWritten);
            Console.WriteLine("Message:     " + last.MachineName);
        }

        public void CreateCounters()
        {
            string category = "jobs monitoring";
            string jobsCompleted = "jobs completed so far";
            string jobsNotStartedYet = "jobs not started yet";

            if (!PerformanceCounterCategory.Exists(category))
            {
                var collection = new CounterCreationDataCollection();

                collection.Add(new CounterCreationData(jobsCompleted,
                    "Number of jobs completed, including job execution time",
                    PerformanceCounterType.NumberOfItems32));

                collection.Add(new CounterCreationData(jobsNotStartedYet,
                    "jobs that have not exectuted yet",
                    PerformanceCounterType.NumberOfItems32));

                PerformanceCounterCategory.Create(category, "Test category",
                    PerformanceCounterCategoryType.SingleInstance, collection);
            }
        }

        public void WriteCounters()
        {
            string category = "jobs monitoring";
            string jobsCompleted = "jobs completed so far";

            using (var counter = new PerformanceCounter(category, jobsCompleted, string.Empty))
            {
                counter.ReadOnly = false;
                counter.RawValue = 1000;
                counter.Increment();
                counter.IncrementBy(10);

                Console.WriteLine(counter.NextValue()); // 1011
            }
        }

        public void UseStopwatch()
        {
            Stopwatch s = Stopwatch.StartNew();
            File.WriteAllText("text.txt", new string('*', 30000000));
            Console.WriteLine(s.Elapsed); // 00:00:01.4322661
        }
    }
}
