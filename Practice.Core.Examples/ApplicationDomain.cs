using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace Practice.Core.Examples
{
    public class ApplicationDomain
    {
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
    }
}
