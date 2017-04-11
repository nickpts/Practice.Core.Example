using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;
using Practice.Core.Examples.Abstractions;

namespace Practice.Core.Examples
{
    public class ApplicationDomain
    {
        private static object lockobj = new object();
        
        public static void CommunicateWithAnotherDomain()
        {
            var testDomain = AppDomain.CreateDomain("testDomain");
            testDomain.SetData("Message", "string data");

            Customer c = (Customer)testDomain.CreateInstanceAndUnwrap(
                typeof(Customer).Assembly.FullName,
                typeof(Customer).FullName);

            c.InvokeFromDomain();
        }

        public static void PassDataBetweenDomains()
        {
            var testDomain = AppDomain.CreateDomain("testDomain");
            testDomain.SetData("Message", "string data");

            testDomain.DoCallBack(SayMessage);
            AppDomain.Unload(testDomain);
        }

        /// <summary>
        /// Meant to illustrate domain functionality using threads.
        /// </summary>
        public static void MultipleThreadsLogin()
        {
            for (int i = 0; i < 20; i++)
            {
                var thread = new Thread(() =>
                {
                    var setup = new AppDomainSetup();
                    var threadDomain = AppDomain.CreateDomain($"threadDomain: {i}", null, setup);
                    threadDomain.DoCallBack(TestFunction);
                });

                thread.Start();
            }
        }

        public static void TestDomain()
        {
            try
            {
                var appDomainSetup = new AppDomainSetup();
                appDomainSetup.ApplicationName = "test";

                var testDomain = AppDomain.CreateDomain("testDomain", null, appDomainSetup);

                // testDomain.ExecuteAssembly(@"C:\Users\Nick\Dev\test\assemblies\Training.Algorithms.exe");
                testDomain.DoCallBack(new CrossAppDomainDelegate(DisplayMessage));
                testDomain.DomainUnload += DisplayMessage;

                AppDomain.Unload(testDomain);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void DisplayMessage(object sender, EventArgs e)
        {
            Console.WriteLine("Unloading");
        }

        private static void DisplayMessage()
        {
            Console.WriteLine("Test message");
        }

        private static void TestFunction()
        {
            var c = new Customer("Test", "Customer");
            Customer.CurrentUser = "Test Customer";

            Console.WriteLine($"Logged in as: " + Customer.CurrentUser + " on " + AppDomain.CurrentDomain.FriendlyName);
        }

        private static void SayMessage()
        {
            string value = (string)AppDomain.CurrentDomain.GetData("Message");
            Console.WriteLine(value);
        }
    }
}
