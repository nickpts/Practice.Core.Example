using Practice.Core.Examples.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Core.Examples.Comparers
{
    public class PriorityComparer : Comparer<Wish>
    {
        public override int Compare(Wish x, Wish y)
        {
            if (object.Equals(x, y)) return 0; // Fail-safe check
            return x.Priority.CompareTo(y.Priority); // still works if x is null
        }
    }
}
