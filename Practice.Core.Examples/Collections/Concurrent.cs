using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Practice.Core.Examples.Collections
{
    public class Concurrent
    {
        public static object tObject = new object();

        public static void ConcurrentDictionaryPerformance()
        {
            var cd = new ConcurrentDictionary<int, int>();

            var watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                cd[i] = 123;
            }

            int duration = watch.Elapsed.Milliseconds;
            Console.WriteLine(duration);
        }

        public static void StandardDictionaryPerformance()
        {
            var d = new Dictionary<int, int>();

            var watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                lock (tObject)
                {
                    d[i] = 123;
                }
            }

            int duration = watch.Elapsed.Milliseconds;
            Console.WriteLine(duration);
        }
        public static void Placeholder()
        {
            var t = new ConcurrentBag<int>();
            
        }


    }
}
