// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the PROJECTNATIVE_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// PROJECTNATIVE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef PROJECTNATIVE_EXPORTS
#define PROJECTNATIVE_API __declspec(dllexport)
#else
#define PROJECTNATIVE_API __declspec(dllimport)
#endif


struct RKRS_H
{
    int offset; // BID 偏移

    int unk1;
    int unk2;

    short size; // BID 大小
    short count; // BID 数量

    short unk3;

    unsigned short val; // BID 起始值
    //  List<BID_H> _bids;
};


struct BID_H
{
    int length; // 长度
    int offset; // 偏移

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


struct BIDD_H
{
    short width; // 图片宽度
    short height; // 图片高度
    short d3; // 颜色深度
    short d4; // 压缩模式
    unsigned short d5;
    unsigned short d6;
    unsigned short d7;
    char data[0]; // image data
};


struct HEADER_H
{
    int code;
    char txt[4];
};


struct MyStruct2
{
    HEADER_H* h;
    RKRS_H* rkrs;
    BID_H* bid;
    BIDD_H** ppbidd;
};


struct MyStruct
{
    void* pvFile;
    void* hFileMap;
    void* hFile;
    MyStruct2 mys2;
};


#ifdef __cplusplus
extern "C" {
#endif

    PROJECTNATIVE_API MyStruct* rkrs_open_file(const char* pszPathName);
    PROJECTNATIVE_API void rkrs_close_file(MyStruct* mys);
    PROJECTNATIVE_API void rkrs_parse(MyStruct* mys, MyStruct2* mys2);
    PROJECTNATIVE_API void* rkrs_read_image_data(MyStruct* mys, int idx);
    PROJECTNATIVE_API void rkrs_free_image_data(void* img);

#ifdef __cplusplus
}
#endif
