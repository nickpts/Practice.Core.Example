using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Practice.Core.Examples.Concurrency
{
    public class CParallel
    {
        //massive file with 1,500,000 words
        private static readonly string filePath = @"C:\Users\Nick\Dev\docs\asyncioexample.txt";

        public static void ParallelPrimeNumbers()
        {
            var watch = new Stopwatch();
            watch.Start();

            IEnumerable<int> numbers = Enumerable.Range(3, 10000000 - 3);

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

        }

        public static List<string> FindWordsParallel()
        {
            HashSet<string> hashWords = new HashSet<string>(
                File.ReadAllLines(filePath),
                StringComparer.InvariantCultureIgnoreCase);

            List<string> megaSet = hashWords.ToList();

            megaSet[2132] = "randomtestValue";
            megaSet[2223] = "differentWordToFind";

            var watch = Stopwatch.StartNew();
            Console.WriteLine("Starting operation...");
            var filteredWords = megaSet.Where(w => !hashWords.Contains(w)).OrderBy(w => w).ToList();

            Console.WriteLine($"Operation took {watch.Elapsed.Seconds} seconds");

            return filteredWords;
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
    }
}
