using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Practice.Core.Examples.Abstractions;
using System.Diagnostics;

namespace Practice.Core.Examples.Concurrency
{
    public class AdvancedThreading
    {
        private static List<string> ourResourceList = new List<string>();
        private static Random randGen = new Random();

        private static Timer genTimer;
        private static int counter = 0;
        private static int maxTimes = 10;

        static AdvancedThreading()
        {

        }

        public AdvancedThreading()
        {
            randomNumList = new ThreadLocal<List<int>>(() => new List<int>());
            standGen = new ThreadLocal<Random>(() => new Random());
        }

        #region Deadlock

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
                lock (objectToLock)
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

        #endregion

        #region Mutex

        private static Mutex synchronizationMutex = new Mutex();

        public static void UseMutexToSynchronizeAccess()
        {
            for (int i = 0; i < 2; i++)
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

        #endregion

        #region Semaphore

        private static Semaphore semPool;

        public static void SemaphoreSixThreads()
        {
            semPool = new Semaphore(0, 3);

            for (int i = 1; i <= 6; i++)
            {
                var thread = new Thread(() =>
                {
                    DoSomethingMethod(i);
                });

                thread.Start();

                Thread.Sleep(500);
            }

            Thread.Sleep(1000);
            semPool.Release(2);

            Console.ReadLine();
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

        #endregion

        #region ReaderWriterLock

        private static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public static void ReaderWriterLockSyncAccess()
        {
            Thread writerThread = null;

            for (int i = 1; i <= 10; i++)
            {
                object hashCode = 0;
                var readerthread = new Thread(() =>
                {
                    hashCode = ReadGenHashcode(i);
                });

                object randomNumber = 0;
                writerThread = new Thread(() =>
                {
                    randomNumber = ReadGeneratedRandomNumber("writer");
                });

                readerthread.Start();
                writerThread.Start();

                Console.WriteLine($"Hashcode is {hashCode}");
                Console.WriteLine($"Random number is: {randomNumber}");
            }

            Console.ReadLine();
        }

        private static int ReadGenHashcode(int threadNumber)
        {
            rwLock.TryEnterReadLock(500);
            int result = randGen.GetHashCode();
            Console.WriteLine($"Thread {threadNumber} is reading the resource");
            rwLock.ExitReadLock();

            Thread.Sleep(1000);

            return result;
        }

        private static int ReadGeneratedRandomNumber(string threadName)
        {
            rwLock.TryEnterWriteLock(500);
            int result = randGen.Next();
            Console.WriteLine($"Thread {threadName} is modifying the state of the resource");
            rwLock.ExitWriteLock();

            Thread.Sleep(1000);
            return result;
        }

        #endregion

        #region ManualResetEvent

        private static ManualResetEvent manEvent = new ManualResetEvent(false);

        public static void ReleaseMultipleThreadsWithWaitEventHandle()
        {
            Thread firstThreadTobeNotified = new Thread(() =>
            {
                MethodWithManualResetEventToBeNotified();
            });

            Thread secondThreadTobeNotified = new Thread(() =>
            {
                MethodWithManualResetEventToBeNotified();
            });

            firstThreadTobeNotified.Start();
            secondThreadTobeNotified.Start();
            Console.WriteLine("Press key to release handle on two threads at the same time");
            Console.ReadKey();
            manEvent.Set();
        }

        private static void MethodWithManualResetEventToBeNotified()
        {
            Console.WriteLine("Method delegate is waiting");
            manEvent.WaitOne();
            Console.WriteLine($"Method delegate has been signalled to return at {DateTime.Now}");
        }

        #endregion

        #region AutoResetEvent

        private static AutoResetEvent autoEvent = new AutoResetEvent(false);

        public static void ThreadsWaitingOnAutoResetEvent()
        {
            Thread firstThreadTobeNotified = new Thread(() =>
            {
                MethodWithAutoResetEventToBeNotified();
            });

            Thread secondThreadTobeNotified = new Thread(() =>
            {
                MethodWithAutoResetEventToBeNotified();
            });

            firstThreadTobeNotified.Start();
            secondThreadTobeNotified.Start();

            Console.WriteLine("Press key to continue");
            Console.ReadKey();
            autoEvent.Set();
            Thread.Sleep(1000);

            Console.WriteLine("Press key to continue");
            Console.ReadKey();
            autoEvent.Set();
        }

        /// <summary>
        /// A method that just waits
        /// </summary>
        private static void MethodWithAutoResetEventToBeNotified()
        {
            Console.WriteLine("Method delegate is waiting");
            autoEvent.WaitOne();
            Console.WriteLine($"Method delegate has been signalled to return at {DateTime.Now}");
        }

        private static void MethodThatPrintsOnConsole(object state)
        {
            Console.WriteLine("Called method has ran");
            counter++;

            if (counter > maxTimes)
            {
                autoEvent.Set();
                counter = 0;
            }
        }

        public static void StartGenTimer()
        {
            genTimer = new Timer(
                        MethodThatPrintsOnConsole,
                        autoEvent,
                        1000,
                        250);

            autoEvent.WaitOne();
            genTimer.Change(0, 500);
            Console.WriteLine("\nChanging period to .5 seconds.\n");


            Console.ReadLine();
            genTimer.Dispose();
        }

        #endregion

        #region CountDownEvent

        private static CountdownEvent cEvent = new CountdownEvent(3);

        public static void ReleaseThreadsWithCountdownEvent()
        {
            Thread firstThreadTobeNotified = new Thread(() =>
            {
                MethodWithCountdownEventToBeNotified();
            });

            Thread secondThreadTobeNotified = new Thread(() =>
            {
                MethodWithCountdownEventToBeNotified();
            });

            Thread thirdThreadTobeNotified = new Thread(() =>
            {
                MethodWithCountdownEventToBeNotified();
            });

            firstThreadTobeNotified.Start();
            Thread.Sleep(1000);
            secondThreadTobeNotified.Start();
            Thread.Sleep(1000);
            Console.WriteLine("Press key to start third thread and trigger countdown event");
            Console.ReadKey();
            thirdThreadTobeNotified.Start();
            cEvent.Wait();
            Console.WriteLine("Countdown event has been triggered");

        }

        private static void MethodWithCountdownEventToBeNotified()
        {
            Console.WriteLine("Entered method with event");
            cEvent.Signal();
            Console.WriteLine("Received signal from countdown event");
        }

        #endregion

        #region Barrier

        private static Action<Barrier> act = (Barrier b) =>
        {
            Console.WriteLine("Post action");
            Console.WriteLine($"Phase number: {b.CurrentPhaseNumber}");
        };

        private static Barrier bar = new Barrier(3, act);

        public static void BarrierExampleToSynchronizeThreeThreads()
        {
            Console.WriteLine($"Current participant count: {bar.ParticipantCount}");

            new Thread(() => { MethodThatDoesSomethingComplicated(1); }).Start();
            new Thread(() => { MethodThatDoesSomethingComplicated(2); }).Start();
            new Thread(() => { MethodThatDoesSomethingComplicated(3); }).Start();

            Console.ReadLine();
        }

        private static void MethodThatDoesSomethingComplicated(int threadNumber)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.Write($"{i}");
                bar.SignalAndWait();
            }

            //dispose barrier - important!
            bar.Dispose();
        }

        #endregion

        #region ThreadLocal
        private ThreadLocal<List<int>> randomNumList = new ThreadLocal<List<int>>();
        private ThreadLocal<Random> standGen;

        public void TryPopulateList()
        {
            var watch = new Stopwatch();
            watch.Start();
            List<int> randomNumberList = new List<int>();

            for (int i = 0; i < 2000; i++)
            {
                new Thread(() =>
                {
                    int random = standGen.Value.Next();
                    randomNumberList.Add(random);

                }).Start();

                new Thread(() =>
                {
                    int random = standGen.Value.Next();
                    randomNumberList.Add(random);

                }).Start();

            }

            var duration = watch.Elapsed.Seconds;
            int duplicates = randomNumberList.Distinct().Count();

            Console.WriteLine($"Operation took { duration } seconds");
        }

        #endregion ThreadLocal
    }
}
