using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace S33Assets
{
    public static class RK28FS
    {
        private delegate int FS_InitializeDelegate(string imagePath, int arg2, int arg3);
        private delegate int FS_DeInitializeDelegate();
        private delegate int FS_GetLoaderPathDelegate(IntPtr ptr);
        private delegate int FS_WriteFileToImgDelegate(string inputPath, string resPath);
        private delegate int FS_WriteFileToPCDelegate(string resPath, string outputPath);

        private static IntPtr s_ptr;

        private static TDelegate _ccall<TDelegate>(string procName)
        {
            IntPtr funcPtr = PInvoke.GetProcAddress(s_ptr, procName);
            TDelegate func = Marshal.GetDelegateForFunctionPointer<TDelegate>(funcPtr);
            return func;
        }

        public static int FS_Initialize(string imagePath, int arg2, int arg3)
        {
            s_ptr = PInvoke.LoadLibrary("RK28FSDll.dll");
            if (s_ptr == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return _ccall<FS_InitializeDelegate>("FS_Initialize")(imagePath, arg2, arg3);
        }

        public static int FS_DeInitialize()
        {
            int val = _ccall<FS_DeInitializeDelegate>("FS_DeInitialize")();

            _ = PInvoke.FreeLibrary(s_ptr);
            s_ptr = IntPtr.Zero;

            return val;
        }

        public static int FS_GetLoaderPath(IntPtr ptr) => _ccall<FS_GetLoaderPathDelegate>("FS_GetLoaderPath")(ptr);

        public static int FS_WriteFileToImg(string inputPath, string resPath) => _ccall<FS_WriteFileToImgDelegate>("FS_WriteFileToImg")(inputPath, resPath);

        public static int FS_WriteFileToPC(string resPath, string outputPath) => _ccall<FS_WriteFileToPCDelegate>("FS_WriteFileToPC")(resPath, outputPath);
    }
}
