using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Practice.Core.Examples.Abstractions
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SharedData
    {
        public int Value;
        public char Letter;
        public fixed float Numbers[50];
    }
}
