using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Core.Examples.Comparers
{
    public class SurnameComparer : Comparer<string>
    {
        private string Normalise(string input)
        {
            input = input.Trim().ToUpper();
            if (input.StartsWith("MC"))
            {
                input = "MAC" + input.Substring(2);
            }

            return input;
        }

        public override int Compare(string x, string y) => Normalise(x).CompareTo(Normalise(y));
    }
}
