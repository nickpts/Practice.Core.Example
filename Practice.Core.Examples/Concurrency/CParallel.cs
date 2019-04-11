using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Practice.Core.Examples.Concurrency
{
    public class CParallel
    {
        //massive file with 1,500,000 words
        public static readonly string millionLineFilePath = @"C:\Users\Nick\Dev\docs\asyncioexample.txt";
        public static readonly string halfMillionLineFilePath = @"C:\Users\user\Downloads\test.txt";
        public static readonly string smallerFilePath = @"C:\Users\Nick\Dev\docs\asyncioexample2.txt";

        public static void ParallelPrimeNumbers()
        {
            var watch = new Stopwatch();
            watch.Start();

            IEnumerable<int> numbers = Enumerable.Range(3, 10000000 - 3);

            Console.WriteLine("Starting parallelism with all cores");

            var parallelQuery = from n in numbers.AsParallel()
                                where Enumerable.Range(2, (int)Math.Sqrt(n)).All(i => n % i > 0)
                                select n;

            // should see all cores working to execute this
            int[] primes = parallelQuery.ToArray();

            Console.WriteLine($"Elapsed time in seconds: { watch.Elapsed.Seconds }");
            Console.ReadKey();

            watch.Reset();
            watch.Start();
            Console.WriteLine("Starting parallelism with 2 cores");
            int[] inefficientPrimes = parallelQuery.WithDegreeOfParallelism(2).ToArray();
            Console.WriteLine($"Elapsed time in seconds: { watch.Elapsed.Seconds }");

            Console.ReadLine();
        }

        public static void FindWordsParallel()
        {
            HashSet<string> hashWords = new HashSet<string>(File.ReadAllLines(millionLineFilePath), StringComparer.InvariantCultureIgnoreCase);

            List<string> megaSet = hashWords.ToList();

            megaSet[2132] = "randomtestValue";
            megaSet[2223] = "differentWordToFind";

            var watch = Stopwatch.StartNew();
            Console.WriteLine("Starting operation...");
            megaSet.Where(w => !hashWords.Contains(w)).OrderBy(w => w).AsParallel().ToList();

            Console.WriteLine($"PLINQ operation took {watch.Elapsed.Milliseconds} milliseconds");
            Console.ReadLine();
        }

        public static void ParallelPrimeNumbersWithCancellation()
        {
            var watch = new Stopwatch();
            watch.Start();

            IEnumerable<int> numbers = Enumerable.Range(3, 10000000 - 3);

            var parallelQuery = from n in numbers.AsParallel()
                                where Enumerable.Range(2, (int)Math.Sqrt(n)).All(i => n % i > 0)
                                select n;

            var source = new CancellationTokenSource();
            var token = source.Token;

            var workerThread = new Thread(() =>
            {
                try
                {
                    int[] primes = parallelQuery.WithCancellation(token).ToArray();
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine($"Query cancelled: { e.Message }");
                }
            });

            workerThread.Start();

            Thread.Sleep(1000);
            //main thread will cancel worker thread operation
            source.Cancel();
        }

        public static void ParallelPrimeNumberInvokation()
        {
            Action firstSet = () => { CalculatePrimerNumbers(3, 10000000, 1); };
            Action secondSet = () => { CalculatePrimerNumbers(10000003, 10000000 / 2, 2); };
            Action thirdSet = () => { CalculatePrimerNumbers(20000006, 10000000 / 3, 3); };

            Parallel.Invoke(firstSet, secondSet, thirdSet);
        }

        public static void CalculatePrimerNumbers(int start, int count, int actionNumber)
        {
            var watch = Stopwatch.StartNew();

            IEnumerable<int> numbers = Enumerable.Range(start, count - 3);

            var parallelQuery = from n in numbers
                                where Enumerable.Range(2, (int)Math.Sqrt(n)).All(i => n % i > 0)
                                select n;

            Console.WriteLine($"Action: { actionNumber } is starting");
            parallelQuery.ToArray();
            Console.WriteLine($"Action: { actionNumber } finished in { watch.ElapsedMilliseconds } milliSeconds");
        }

        public static void ParallelKeyGeneration()
        {
            var watch = Stopwatch.StartNew();

            var newKeyPairs = new string[6];
            Parallel.For(1, newKeyPairs.Length, (i, loopstate) =>
            {
                newKeyPairs[i] = RSA.Create().ToXmlString(true);
                Console.WriteLine($"Generating key pair: {i} ");

                if (i == 5)
                {
                    // will break out of the loop
                    // loopstate.Break(); 
                }
            });

            Console.WriteLine($"Operation finished in { watch.ElapsedMilliseconds } milliSeconds");

            watch.Reset();
            watch.Start();

            var keyPairs = new string[6];
            for (int i = 0; i <= 5; i++)
            {
                keyPairs[i] = RSA.Create().ToXmlString(true);
            }

            // same thing can be done with parallel linq
            // string[] keyPairs = ParallelEnumerable.Range(0, 6).Select(i => RSA.Create().ToXmlString(true)).ToArray();

            //parallel foreach is much more efficient
            Console.WriteLine($"Operation finished in { watch.ElapsedMilliseconds } milliSeconds");
        }

        public static void TaskFactoryAsyncStateExample()
        {
            Action<object> act = (o) => { };

            // initial state object will be passed to the delegate
            var t = Task.Factory.StartNew(act, "test state object");
            Console.WriteLine(t.AsyncState);
        }

        public static void TaskWithChild()
        {
            Action act = () =>
            {
                var watch = Stopwatch.StartNew();

                Console.WriteLine("Working on main task");
                List<string> allLines = File.ReadAllLines(smallerFilePath).ToList();
                Console.WriteLine($"Completed main task in { watch.ElapsedMilliseconds } milliseconds");

                Console.WriteLine("Working on child task");
                watch.Reset();
                watch.Start();

                var cTask = Task.Factory.StartNew(() =>
                {
                    List<string> fullText = File.ReadAllLines(millionLineFilePath).ToList();
                }, TaskCreationOptions.AttachedToParent);

                Console.WriteLine($"Completed child task in { watch.ElapsedMilliseconds } milliseconds");
            };

            var pTask = Task.Factory.StartNew(act);
        }

        public static string TaskCombinedString()
        {
            Func<string> randomTrans = () => { return "Entered task, "; };

            Task<string> stringTransformationTask = Task.Factory.StartNew(randomTrans)
                .ContinueWith((s) => { return s.Result + "modified by continuation task 1, "; })
                .ContinueWith((s) => { return s.Result + "modified by continuation task 2"; });

            return stringTransformationTask.Result.ToString();
        }

        public static void GetExceptionsFromChildTasks()
        {
            var options = TaskCreationOptions.AttachedToParent;

            Task randomTask = Task.Factory.StartNew(() =>
            {

                Task.Factory.StartNew(() => { throw new InvalidOperationException(); }, options)
                .ContinueWith(p => { throw new FieldAccessException(); }, TaskContinuationOptions.AttachedToParent);

                Task.Factory.StartNew(() => { throw new NullReferenceException(); }, options);
                Task.Factory.StartNew(() => { throw new InvalidCastException(); }, options);

            }).ContinueWith(p => 
            {
                var ex = p.Exception.Flatten(); // will return 4 exceptions


                p.Exception.Flatten().Handle(aex =>
                {
                    if (aex is FieldAccessException)
                    {
                        return true; // this exception is handled
                    }
                    else return false; // should rethrow three other exceptions
                 });

            }, TaskContinuationOptions.OnlyOnFaulted);

            randomTask.Wait();
        }
    }
}
