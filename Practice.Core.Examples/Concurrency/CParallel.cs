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
        private static readonly string millionLineFilePath = @"C:\Users\Nick\Dev\docs\asyncioexample.txt";
        private static readonly string halfMillionLineFilePath = @"C:\Users\user\Downloads\test.txt";

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
                File.ReadAllLines(millionLineFilePath),
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
    }
}
