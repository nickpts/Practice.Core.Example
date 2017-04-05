using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Practice.Core.Examples.Abstractions;

namespace Practice.Core.Examples
{
    public class Interop
    {
        [DllImport("user32.dll")]
        static extern int MessageBox(IntPtr hWnd, string text, string caption, int type);

        [DllImport("kernel32.dll")]
        static extern int GetWindowsDirectory(StringBuilder builder, int maxChars);

        [DllImport("kernel32.dll")]
        static extern void GetSystemTime(SystemTime t);

        // UnmanagedType contains various enum flags
        // static extern int Test([MarshalAs(UnmanagedType.LPTStr)]string s);

        public static void DisplayInteropMessageBox()
        {
            MessageBox(IntPtr.Zero, "Please do not press this again", "Attention", 0);
        }

        public static void DisplayWindowsDirectory()
        {
            var builder = new StringBuilder(256);
            GetWindowsDirectory(builder, 256);
            Console.WriteLine(builder.ToString());
        }

        public static void DisplaySystemTime()
        {
            var sysTime = new SystemTime();
            GetSystemTime(sysTime);
            Console.WriteLine(sysTime.Month);
        }
    }
}
