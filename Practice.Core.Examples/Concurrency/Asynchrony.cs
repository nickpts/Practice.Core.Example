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
    public class Asynchrony
    {
        private static bool done;
        private static readonly object locker = new object();

        public static void Main()
        {

        }

        public static async void TaskWithSimpleCancellation()
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            Task<int> readExampleFile = ReturnFileLengthAsync(@"C:\Users\Nick\Dev\docs\asyncioexample.txt")
                .WithCancellationToken<int>(token);

            source.CancelAfter(new TimeSpan(0, 0, 5));

            // task will be cancelled before result is returned
            int result = await readExampleFile;
        }

        public static void TryWithTimeout()
        {
            Task<List<string>> readFirstExampleFile = ReadFileAsync(@"C:\Users\Nick\Dev\docs\asyncioexample.txt")
                .WithTimeout<List<string>>(new TimeSpan(0, 0, 5));

            var result = readFirstExampleFile.Result; //will throw AggregateException containing the timeout exception
        }

        public static async void TaskCombinationsSyncInt()
        {
            Task<int> readFirstExampleFile = ReturnFileLengthAsync(@"C:\Users\Nick\Dev\docs\asyncioexample2.txt");
            Task<int> readSecondExampleFile = ReturnFileLengthTestAsync(@"C:\Users\Nick\Dev\docs\asyncioexample.txt");

            int[] t = await Task.WhenAll(readFirstExampleFile, readSecondExampleFile);

            var firstCompletedTaskResult = t[0];
        }

        public static async void TaskCombinationsAsync()
        {
            Task<List<string>> readFirstExampleFile = ReadFileAsync(@"C:\Users\Nick\Dev\docs\asyncioexample2.txt");
            Task<List<string>> readSecondExampleFile = ReadFileAsyncTest(new CancellationToken(), @"C:\Users\Nick\Dev\docs\asyncioexample.txt");

            // at this point control will return to the main thread.
            var t = await Task.WhenAll(readFirstExampleFile, readSecondExampleFile);

            var result = t.First();
        }

        private void Go()
        {
            lock (locker)
            {
                if (!done) { Console.WriteLine("Done"); done = true; }
            }
        }

        /* Output: xxxxxxxxxxxxyyyyyyyyyyyyyyyy
         * xxxxxxxxxxxxyyyyyyyyyyyyyyyyyyyyyyyy
         * xxxxxxxxxxxyyyyyyyyyyyyyyyyyyyyyyyyy
         * yyyyyyyxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
         */

        public static void DoSomething()
        {
            Task<string> someTask = new Task<string>(delegate
            {
                return "Hi, I am Nick";
            });

            someTask.ContinueWith(x => { Console.WriteLine($"{x.Result} and I am here to chew bugglegum"); });

            someTask.Start();
            someTask.Wait();
        }

        public static async void LoopExampleAsync()
        {
            // call asynchronous I/O bound operation
            Task<List<string>> testFile = ReadFileAsync(@"C:\Users\Nick\Dev\docs\asyncioexample.txt");

            // CPU bound operation - 52 seconds on single i7 core @ 2.62 GHz
            for (double d = 0; d < 10000000000; d++)
            {
                // while CPU is busy in this loop, I/O operation is running 
                // on the background
            }

            // by the time the CPU-bound operation is finished
            // the I/O operation has finished too, so the below call returns immediately.
            List<string> fileLine = await testFile;
        }

        public static async Task<int> ReturnFileLengthAsync(string file)
        {
            var task = await ReadFileAsync(file);

            return task.Count();
        }

        public static async Task<int> ReturnFileLengthTestAsync(string file)
        {
            var task = await ReadFileAsyncTest(new CancellationToken(), file);

            return task.Count();
        }

        /// <summary>
        /// I/O bound operation meant to demonstrate asynchrony
        /// </summary>
        public static async Task<List<string>> ReadFileAsync(string file)
        {
            string fileText;

            using (StreamReader reader = File.OpenText(file))
            {
                var stop = Stopwatch.StartNew();
                fileText = await reader.ReadToEndAsync();
                int secElapsed = stop.Elapsed.Seconds; // 27 seconds on SATA 1.5gbps SSD to read 130mb file
            }

            var col = new List<string>();
            col.Add(fileText);

            return col;
        }

        public static async void ReadFileAsyncCancellable()
        {
            CancellationTokenSource source = new CancellationTokenSource();

            // passing the token like this will only cancel the task if 
            // token was cancelled before the task was started. 
            Task<List<string>> file = ReadFileAsyncTest(source.Token, @"C:\Users\Nick\Dev\docs\asyncioexample.txt");

            for (double d = 0; d < 100000; d++)
            {
                if (file.IsCanceled)
                {
                    Console.WriteLine("Task cancelled!");
                    // isCompleted = true;
                    // isCancelled = true;
                    // Exception = null;
                }

                if (d == 500)
                {
                    source.Cancel();
                }
            }
        }

        /// <summary>
        /// For the task to be cancelled, the state of the token must be examined within the method
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<List<string>> ReadFileAsyncTest(CancellationToken token, string file)
        {
            List<string> lines = new List<string>();

            using (var reader = File.OpenText(file))
            {
                while (reader.Peek() > -1)
                {
                    //can also use IsCancellationRequested property
                    token.ThrowIfCancellationRequested();
                    string line = await reader.ReadLineAsync();
                    lines.Add(line);
                }
            }

            return lines;
        }

        public static async Task<List<string>> ReadWholeFileAsync(string fileUri, CancellationToken token)
        {
            List<string> lines = new List<string>();

            using (TextReader rw = new StreamReader(fileUri))
            {
                while (rw.Read() > -1)
                {
                    if (!token.IsCancellationRequested)
                    {
                        string line = await rw.ReadLineAsync();
                        lines.Add(line);
                    }
                    else return lines;
                }
            }

            return lines;
        }
    }
}
