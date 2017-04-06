using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Practice.Core.Examples.Interop;

using Share = Practice.Core.Examples.Interop.SharedMemory;

namespace Practice.Core.SharedMemory
{
    public class Program
    {
        static void Main(string[] args)
        {
            // allocates 1mb of shared memory
            using (var share = new Share("MyShare", false, 1000000))
            {
                // pointer to shared memory
                IntPtr root = share.Root;            

                Console.ReadLine();
            }
        }
    }
}
