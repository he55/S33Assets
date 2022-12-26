using System;
using System.Runtime.InteropServices;

namespace S33Assets
{
    internal static class PInvoke
    {
        [DllImport("ProjectNative.dll")]
        public static extern IntPtr rkrs_open_file(string pszPathName);
        [DllImport("ProjectNative.dll")]
        public static extern void rkrs_close_file(IntPtr _this);
        [DllImport("ProjectNative.dll")]
        public static extern void rkrs_parse(IntPtr _this, out _MyStruct2 mys2);
        [DllImport("ProjectNative.dll")]
        public static extern void rkrs_read_bidd_h(IntPtr _this, int idx, out _BIDD_H pbidd);
        [DllImport("ProjectNative.dll")]
        public static extern IntPtr rkrs_read_image_data(IntPtr _this, int idx);
        [DllImport("ProjectNative.dll")]
        public static extern void rkrs_free_image_data(IntPtr img);
    }


    public struct _RKRS_H
    {
        public int offset; // BID 偏移
        public int re1;
        public int re2;
        public short size; // BID 大小
        public short count; // BID 数量
        public short r3;
        public ushort val; // BID 起始值
                           //  List<BID_H> _bids;
    };


    public struct _BID_H
    {
        public int length; // 长度
        public int offset; // 偏移

        /*
            //int _bindex;
            //int _bid;
            //  BIDD_H _bidd;
                 string Id => $"BID_{_bid:X}";
                 string Name => "";
                 string Size => $"{_bidd.width}*{_bidd.height}";
                 string Offset => $"0x{offset:X}";
                 string Length => $"{length - 14}";
                 string D3 => $"0x{_bidd.d3:X}";
                 string D4 => $"0x{_bidd.d4:X}";
                 string D5 => $"0x{_bidd.d5:X}";
                  */
    };


    public struct _BIDD_H
    {
        public short width; // 图片宽度
        public short height; // 图片高度
        public short d3; // 颜色深度
        public short d4; // 压缩模式
        public ushort d5;
        public ushort d6;
        public ushort d7;
        public IntPtr data; // data image
    }


    public struct _HEADER_H
    {
        public int code;
        public int txt;
    }


    public unsafe struct _MyStruct2
    {
        public _HEADER_H* h;
        public _RKRS_H* rkrs;
        public _BID_H* bid;
        public _BIDD_H** ppbidd;
    }

}
