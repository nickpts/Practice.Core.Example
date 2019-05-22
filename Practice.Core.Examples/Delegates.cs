using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Core.Examples
{
	class Delegates
	{
		public void RunAction()
		{
			Action<int> example1 = (x) => Console.WriteLine("Write {0}", x);
			Action<int, int> example2 = (x, y) => Console.WriteLine("Write {0} and {1}", x, y);
			Action example3 = () => Console.WriteLine("Done");

			example1.Invoke(1);
			example2.Invoke(1, 1);
			example3.Invoke();

			Action<int> example4 = new Action<int>(MethodThatPrintsToConsole);
			example4.Invoke(4);
		}

		public void RunFunc()
		{

			Func<int, int, int> sum = MethodThatAdds;

			int result = sum.Invoke(1, 2);

			Func<int> getRandomNumber = () => 
			{
				Random rnd = new Random();
				return rnd.Next(1, 100);
			};

			int rand = getRandomNumber.Invoke();


		}

		public void MethodThatPrintsToConsole(int param)
		{
			Console.WriteLine("Write {0}", param);
		}

		public int MethodThatAdds(int x, int y)
		{
			return x + y;
		}
	}
}
