using Practice.Core.Examples.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Core.Examples.Comparers
{
    public class FirstLastEqComparer : EqualityComparer<Customer>
    {
        public override bool Equals(Customer x, Customer y) =>
            x.FirstName == y.FirstName && x.LastName == y.LastName;

        public override int GetHashCode(Customer obj) =>
            (obj.FirstName + ";" + obj.LastName).GetHashCode();
    }
}
