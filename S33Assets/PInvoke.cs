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

        [DllImport("Project2.dll")]
        internal static extern IntPtr rkrs_open_file(string pszPathName);
        [DllImport("Project2.dll")]
        internal static extern void rkrs_close_file(IntPtr mys);
        [DllImport("Project2.dll")]
        internal static extern void rkrs_parse(IntPtr mys, out Form1._MyStruct2 mys2);
        [DllImport("Project2.dll")]
        internal static extern IntPtr rkrs_read_image_data(IntPtr mys, int idx);
        [DllImport("Project2.dll")]
        internal static extern IntPtr rkrs_free_image_data(IntPtr img);

    }
}
