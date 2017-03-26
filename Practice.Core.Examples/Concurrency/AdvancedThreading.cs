using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Practice.Core.Examples.Concurrency
{     
    public class AdvancedThreading
    {
        private static Mutex synchronizationMutex = new Mutex();
        private static List<string> ourResourceList = new List<string>();
        private static Semaphore semPool;

        public static void UseMutexToSynchronizeAccess()
        {
            for(int i = 0; i < 2; i++)
            {
                var newThread = new Thread(UseResource);
                newThread.Name = "thread" + i;
                newThread.Start();
            }

            Thread.Sleep(10000);

            synchronizationMutex.Dispose();
            // needs to be released otherwise an exception
            // will be thrown if WaitOne is called
            synchronizationMutex.ReleaseMutex(); 
        }

        /// <summary>
        /// A method using a resource that must be synchronized
        /// </summary>
        private static void UseResource()
        {
            Console.WriteLine($"Thread: {Thread.CurrentThread.Name} is using the resource");
            synchronizationMutex.WaitOne();

            string threadName = Thread.CurrentThread.Name;

            for (int i = 0; i < 10001; i++)
            {
                ourResourceList.Add(threadName + "iteration" + i);
            }

            Console.WriteLine($"Thread: {Thread.CurrentThread.Name} finished using the resource");

            synchronizationMutex.ReleaseMutex();

            Console.WriteLine($"Thread: {Thread.CurrentThread.Name} has released the mutex successfully");
        }

        public static void DeadlockExample()
        {
            string stringToLock = string.Empty;
            object objectToLock = new object();

            var firstThread = new Thread(() =>
            {
                lock (stringToLock)
                {
                    Console.WriteLine("string locked by first thread!");
                    Thread.Sleep(1000);

                    lock (objectToLock)
                    {
                        // we will never get here because of the deadlock
                        Console.WriteLine("object locked!");
                    }
                }
            });

            var secondThread = new Thread(() =>
            {
                lock(objectToLock)
                {
                    Console.WriteLine("object locked by second thread!");

                    lock (stringToLock)
                    {
                        // we will never get here because of the deadlock
                        Console.WriteLine("string locked!");
                    }
                }
            });

            firstThread.Start();
            secondThread.Start();

            Thread.Sleep(10000);
            Console.WriteLine("Reached the end of the method");
        }

        public static void SemaphoreSixThreads()
        {
            semPool = new Semaphore(0, 3);

            for(int i = 1; i <= 6; i++)
            {
                var thread = new Thread(() =>
                {
                    DoSomethingMethod(i);
                });

                thread.Start();

                Thread.Sleep(500);
            }

            Thread.Sleep(1000);
            semPool.Release(1);
        }

        private static void DoSomethingMethod(int num)
        {
            Console.WriteLine("Thread {0} begins " + "and waits for the semaphore.", num);
            semPool.WaitOne();
            Thread.Sleep(1000);
            Console.WriteLine("Thread {0} does something completely pointless", num);

            Console.WriteLine("Thread {0} releases the semaphore.", num);
            Console.WriteLine("Thread {0} previous semaphore count: {1}", num, semPool.Release());
        }
    }
}
