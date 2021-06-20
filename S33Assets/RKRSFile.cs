using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace S33Assets
{
    public static class RKRSFile
    {
        public static RKRS_H ReadFile(string filePath)
        {
            using (FileStream fileStream = File.OpenRead(filePath))
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                if (binaryReader.ReadInt32() != 0x00000028 || binaryReader.ReadInt32() != 0x53524b52)
                {
                    throw new Exception("二进制文件格式错误");
                }

                RKRS_H rkrs = new RKRS_H();
                rkrs._bids = new List<BID_H>();

                fileStream.Position = 0x30;
                rkrs.offset = binaryReader.ReadInt32();

                fileStream.Position = 0x3c;
                rkrs.size = binaryReader.ReadInt16();
                rkrs.count = binaryReader.ReadInt16();

                fileStream.Position = 0x42;
                rkrs.val = binaryReader.ReadUInt16();

                fileStream.Position = rkrs.offset;

                for (int i = 0; i < rkrs.count; i++)
                {
                    BID_H bid = new BID_H();
                    bid.length = binaryReader.ReadInt32();
                    bid.offset = binaryReader.ReadInt32();
                    bid._bindex = i;
                    bid._bid = rkrs.val + i;
                    rkrs._bids.Add(bid);
                }

                foreach (BID_H bid in rkrs._bids)
                {
                    fileStream.Position = bid.offset;

                    BIDD_H bidd = new BIDD_H();
                    bid._bidd = bidd;

                    bidd.width = binaryReader.ReadInt16();
                    bidd.height = binaryReader.ReadInt16();
                    bidd.d3 = binaryReader.ReadInt16();
                    bidd.d4 = binaryReader.ReadInt16();
                    bidd.d5 = binaryReader.ReadUInt16();
                    bidd.d6 = binaryReader.ReadUInt16();
                    bidd.d7 = binaryReader.ReadUInt16();
                }
                return rkrs;
            }
        }

        public static Bitmap ReadBitmap(string filePath, int index)
        {
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                return ReadBitmap(fileStream, index);
            }
        }

        public static Bitmap ReadBitmap(Stream stream, int index)
        {
            stream.Position = 0;

            BinaryReader binaryReader = new BinaryReader(stream);
            if (binaryReader.ReadInt32() != 0x00000028 || binaryReader.ReadInt32() != 0x53524b52)
            {
                throw new Exception("二进制文件格式错误");
            }

            RKRS_H rkrs = new RKRS_H();

            stream.Position = 0x30;
            rkrs.offset = binaryReader.ReadInt32();

            stream.Position = 0x3c;
            rkrs.size = binaryReader.ReadInt16();
            rkrs.count = binaryReader.ReadInt16();

            stream.Position = 0x42;
            rkrs.val = binaryReader.ReadUInt16();

            stream.Position = rkrs.offset + rkrs.size * index;

            BID_H bid = new BID_H();
            bid.length = binaryReader.ReadInt32();
            bid.offset = binaryReader.ReadInt32();
            bid._bindex = index;
            bid._bid = rkrs.val + index;

            stream.Position = bid.offset;

            BIDD_H bidd = new BIDD_H();
            bid._bidd = bidd;

            bidd.width = binaryReader.ReadInt16();
            bidd.height = binaryReader.ReadInt16();
            bidd.d3 = binaryReader.ReadInt16();
            bidd.d4 = binaryReader.ReadInt16();
            bidd.d5 = binaryReader.ReadUInt16();
            bidd.d6 = binaryReader.ReadUInt16();
            bidd.d7 = binaryReader.ReadUInt16();

            Bitmap bitmap = new Bitmap(bid._bidd.width, bid._bidd.height);

            if (bid._bidd.d4 == 1)
            {
                int x = 0;
                int y = 0;

                while (true)
                {
                    uint v = binaryReader.ReadUInt32();
                    if (v == 0)
                    {
                        break;
                    }

                    uint val = v & 0x00ffffff;
                    if (val != 0x00ff00ff)
                    {
                        val |= 0xff000000;
                    }

                    uint len = v >> 24;
                    if (len == 0xff)
                    {
                        len += binaryReader.ReadUInt32();
                    }

                    for (int i = 0; i < len; i++)
                    {
                        bitmap.SetPixel(x, y, Color.FromArgb((int)val));
                        if (++x == bid._bidd.width)
                        {
                            x = 0;
                            ++y;
                        }
                    }
                }

                Debug.Assert(y == bid._bidd.height);
            }
            else if (bid._bidd.d4 == 0)
            {
                for (int y = 0; y < bid._bidd.height; y++)
                {
                    for (int x = 0; x < bid._bidd.width; x++)
                    {
                        uint val = binaryReader.ReadUInt32();
                        if (val != 0x00ff00ff)
                        {
                            val |= 0xff000000;
                        }
                        bitmap.SetPixel(x, y, Color.FromArgb((int)val));
                    }
                }
            }
            else if (bid._bidd.d4 == 5)
            {
                uint alpha = 0;
                if (bid._bidd.d5 == 3)
                {
                    alpha = 0xff000000;
                }

                for (int y = 0; y < bid._bidd.height; y++)
                {
                    for (int x = 0; x < bid._bidd.width; x++)
                    {
                        uint val = binaryReader.ReadUInt32();
                        val |= alpha;
                        bitmap.SetPixel(x, y, Color.FromArgb((int)val));
                    }
                }
            }

            return bitmap;
        }
    }

    /// <summary>
    /// RKRS_H
    /// </summary>
    public class RKRS_H
    {
        /// <summary>
        /// BID 偏移
        /// </summary>
        public int offset;

        /// <summary>
        /// BID 大小
        /// </summary>
        public short size;

        /// <summary>
        /// BID 数量
        /// </summary>
        public short count;

        /// <summary>
        /// BID 起始值
        /// </summary>
        public ushort val;
        public List<BID_H> _bids;
    }

    /// <summary>
    /// BID_H
    /// </summary>
    public class BID_H
    {
        /// <summary>
        /// 长度
        /// </summary>
        public int length;

        /// <summary>
        /// 偏移
        /// </summary>
        public int offset;
        public int _bindex;
        public int _bid;
        public BIDD_H _bidd;

        public string Id => $"BID_{_bid:X}";
        public string Name => "";
        public string Size => $"{_bidd.width}*{_bidd.height}";
        public string Offset => $"0x{offset:X}";
        public string Length => $"{length - 14}";
        public string D3 => $"0x{_bidd.d3:X}";
        public string D4 => $"0x{_bidd.d4:X}";
        public string D5 => $"0x{_bidd.d5:X}";
    }

    /// <summary>
    /// BIDD_H
    /// </summary>
    public class BIDD_H
    {
        /// <summary>
        /// 图片宽度
        /// </summary>
        public short width;

        /// <summary>
        /// 图片高度
        /// </summary>
        public short height;

        /// <summary>
        /// 颜色深度
        /// </summary>
        public short d3;

        /// <summary>
        /// 压缩模式
        /// </summary>
        public short d4;
        public ushort d5;
        public ushort d6;
        public ushort d7;
    }
}
