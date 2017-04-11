using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Practice.Core.Examples.Interop;
using Practice.Core.Examples.Abstractions;
using Share = Practice.Core.Examples.Interop.SharedMemory;
using System.IO.MemoryMappedFiles;

namespace Practice.Core.SharedMemoryReaderWriter
{
    public class Program
    {
        public unsafe static void Main(string[] args)
        {
            using (var mapper = MemoryMappedFile.CreateNew("MyShare", 1000000))
            using (var accessor = mapper.CreateViewAccessor())
            {
                byte* pointer = null;
                accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);

                void* root = pointer;
                SharedData* data = (SharedData*)root;

                Console.WriteLine("Value is " + data->Value);
                Console.WriteLine("Letter is " + data->Letter);
                Console.WriteLine("11th Number is " + data->Numbers[10]);

                // Our turn to update values in shared memory!
                data->Value++;
                data->Letter = '!';
                data->Numbers[10] = 987.5f;

                Console.WriteLine("Updated shared memory");
                Console.ReadLine();
            }
        }
    }
}
