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

        #region C# 6.0

        // read-only auto property
        public string TestReadOnlyProperty => string.Empty;

        // automatic initialization for auto property
        public string TestProperty { get; set; } = string.Empty;

        public static void CSharp6()
        {
            Stock item = new Stock("*");
            var name = nameof(item);

            // will display "item";
            Console.WriteLine(name);

            item = null;
            // null conditional operator
            Console.Write(item?.Price); // will write nothing and will not throw exception
        }

        // expression-bodied method with return type
        public string ReturnSomethingInteresting() => string.Empty;

        // expression-bodied method with no return type
        public void DoSomethingInteresting() => Console.WriteLine("test");

        #endregion

        #region C# 7.0

        // _ can now appear as a separator in literals
        int x = 123_456;

        // binary literal - no need to know remember hexadecimal
        byte b1 = 0b00101101;

        // expression-bodied constructor
        public LangFeatures(string inputVariable) => inputVariable = "Test";

        // expression-bodied constructor with throw statement
        public LangFeatures(string inputVariable, string testVariable) => TestProperty = testVariable ?? throw new ArgumentException(nameof(testVariable));

        // expression-bodied property
        public string TestExpProp
        {
            get => "test";
            set => TestProperty = value;
        }

        // expression-bodied method with throw
        public string TestThrowMethod() => throw new NotImplementedException();

        // throw expression
        public string TestTernaryWithThrow(string testInput)
        {
            return testInput.Length > 0 ? testInput : throw new InvalidOperationException();
        }
        

        #endregion

        #region Private implementation

        private static void TestOutMethod(int x, int y)
        {

        }

        #endregion

    }
}
