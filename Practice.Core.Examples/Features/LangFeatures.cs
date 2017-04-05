using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Practice.Core.Examples.Abstractions;
using Practice.Core.Examples.Collections;
using Practice.Core.Examples.Comparers;

namespace Practice.Core.Examples.Features
{
    public class LangFeatures
    {
        public LangFeatures() { }

        // read-only auto property
        public string TestReadOnlyProperty => string.Empty;

        // automatic initialization for auto property
        public string TestProperty { get; set; } = string.Empty;


        public static void CSharp6()
        {
            Stock item = new Stock("*");
            var name = nameof(item);

            //will display "item";
            Console.WriteLine(name);

            item = null;
            // null conditional operator
            Console.Write(item?.Price); // will write nothing and will not throw exception
        }

        // expression-bodied method with return type
        public string ReturnSomethingInteresting() => string.Empty;

        // expression-bodied void method 
        public void DoSomethingInteresting() => Console.WriteLine("test"); 
    }
}
