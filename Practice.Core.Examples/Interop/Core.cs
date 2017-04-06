using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Practice.Core.Examples.Abstractions;

namespace Practice.Core.Examples.Interop
{
    public class Core
    {
        [DllImport("user32.dll")]
        static extern int MessageBox(IntPtr hWnd, string text, string caption, int type);

        [DllImport("kernel32.dll")]
        static extern int GetWindowsDirectory(StringBuilder sb, int maxChars);

        [DllImport("kernel32.dll")]
        static extern void GetSystemTime(out SystemTime t);

        // needs to have the same signature as:
        // BOOL CALLBACK EnumWindowsProc (HWND hwnd, LPARAM lParam);
        private delegate bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam);

        // UnmanagedType contains various enum flags
        // static extern int Test([MarshalAs(UnmanagedType.LPTStr)]string s);

        public static void DisplayInteropMessageBox()
        {
            MessageBox(IntPtr.Zero, "Please do not press this again", "Attention", 0);
        }

        [DllImport("user32.dll")]
        static extern int EnumWindows(EnumWindowsCallback hWnd, IntPtr lParam);

        public static void DisplaySystemTime()
        {
            var time = new SystemTime();
            GetSystemTime(out time);

            Console.WriteLine($"Time is: { time.Hour }:{ time.Minute }:{ time.Second }");
        }

        public static void DisplayWindowsDirectory()
        {
            var builder = new StringBuilder(256);
            GetWindowsDirectory(builder, 256);

            Console.WriteLine(builder.ToString());
        }
        
        public static void WindowHandleCallback() => EnumWindows(PrintWindow, IntPtr.Zero);

        private static bool PrintWindow(IntPtr hWnd, IntPtr lParam)
        {
            Console.WriteLine(hWnd.ToInt64());
            return true;
        }

        public unsafe static void AllocateUnsafeOnStack()
        {
            // allocates memory on the stack for data
            SharedData* data = stackalloc SharedData[1];

            data->Value = 123;
            data->Letter = 'X';
            data->Numbers[10] = 1.45F;

            Console.WriteLine(data->Value);
        }

        public unsafe static void AllocateUnsafeOnUnmanagedHeap()
        {
            // allocates memory on the unmanaged heap
            SharedData* data = (SharedData*)Marshal.AllocHGlobal(sizeof(SharedData)).ToPointer();

            data->Value = 123;
            data->Letter = 'X';
            data->Numbers[10] = 1.45f;

            // need to de-allocate memory once finished, otherwise we get a memory leak
            Marshal.FreeHGlobal(new IntPtr(data));
        }
    }
}
