using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Security;
using System.IO;
using System.Security.Permissions;

namespace Practice.Core.Examples
{
    public class StringFormatExamples
    {
        public static void WriteSomeStuff()
        {
            int currency = 9;
            string cString = currency.ToString("C2");

            double value = 2.345;
            string cValue = value.ToString(".##");

            var date = DateTime.Now;
            string sDate = date.ToString("u");
        }
    }
}
