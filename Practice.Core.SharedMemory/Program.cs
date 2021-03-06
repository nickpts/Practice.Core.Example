﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Practice.Core.Examples.Interop;
using Practice.Core.Examples.Abstractions;
using Share = Practice.Core.Examples.Interop.SharedMemory;

namespace Practice.Core.SharedMemory
{
    public class Program
    {
        public unsafe static void Main(string[] args)
        {
            // allocates 1mb of shared memory
            using (var share = new Share("MyShare", false, 1000000))
            {
                void* root = share.Root.ToPointer();
                SharedData* data = (SharedData*)root;

                data->Value = 123;
                data->Letter = 'X';
                data->Numbers[10] = 1.45f;

                Console.WriteLine("Data written to shared memory");
                Console.ReadLine();

                Console.WriteLine("Value is " + data->Value);
                Console.WriteLine("Letter is " + data->Letter);
                Console.WriteLine("11th Number is " + data->Numbers[10]);
                Console.ReadLine();
            }
        }
    }
}
