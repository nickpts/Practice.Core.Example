using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Practice.Core.Examples.Abstractions
{
    public class Customer : MarshalByRefObject
    {
        // static field that would interfere with other client logins
        // if running in the same app domain.
        public static string CurrentUser = "";

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Customer(string first, string last)
        {
            FirstName = first;
            LastName = last;
        }

        public static void Login(string name, string password)
        {
            if (CurrentUser.Length == 0) // If we're not already logged in...
            {
                // Sleep to simulate authentication...
                Thread.Sleep(500);
                CurrentUser = name; // Record that we're authenticated.
            }
        }

        public void InvokeFromDomain() =>
            Console.WriteLine($"Message from domain: { AppDomain.CurrentDomain.FriendlyName }");

        public override object InitializeLifetimeService() => null;
    }
}
