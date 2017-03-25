using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Core.Examples.Comparers
{
    public abstract class EqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
    {
        public abstract bool Equals(T x, T y);

        public abstract int GetHashCode(T obj);

        bool IEqualityComparer.Equals(object x, object y)
        {
            throw new NotImplementedException();
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        public static EqualityComparer<T> Default { get; }
    }

}
