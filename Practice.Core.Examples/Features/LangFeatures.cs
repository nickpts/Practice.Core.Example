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

        // fluid out variables --  
        private static void CallTestOutMethod()
        {
            TestOutMethod(out int inputX, out int inputY);

            Console.WriteLine(inputX); // 1;
            Console.WriteLine(inputY); // 2;
        }

        private static void CallTestOutMethodWithDiscard()
        {
            TestOutMethod(out int inputX, out _);

            Console.WriteLine(inputX); // 1;
        }

        // expression-bodied method with throw
        public string TestThrowMethod() => throw new NotImplementedException();

        // throw expression
        public string TestTernaryWithThrow(string testInput)
        {
            return testInput.Length > 0 ? testInput : throw new InvalidOperationException();
        }

        // switch statement with pattern
        public static void TestSwithWithPatterns(object o)
        {
            switch (o)
            {
                case Stock s:
                    Console.WriteLine("It's a stock!");
                    break;
                case Wish w when (w.Name == "test"):
                    Console.WriteLine($"It's a wish with priority: { w.Priority }");
                    break;
                default:
                    break;
                case null:
                    throw new ArgumentNullException(nameof(o));
            }
        }

        // method calling local method
        public static void PrintOutVariable()
        {
            Print("calling local function!");

            void Print(string test)
            {
                Console.WriteLine(test);
            }
        }
        

        #endregion

        #region Private implementation

        private static void TestOutMethod(out int x, out int y)
        {
            x = 1;
            y = 2;
        }

        #endregion

    }
}
