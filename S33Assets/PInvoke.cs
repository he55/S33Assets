using System;
using System.Runtime.InteropServices;

namespace S33Assets
{
    internal static class PInvoke
    {
        [DllImport("Kernel32")]
        internal static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("Kernel32")]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("Kernel32")]
        internal static extern int FreeLibrary(IntPtr hLibModule);
    }
}
