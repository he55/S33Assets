// ProjectNative.cpp : Defines the exported functions for the DLL.
//

#include "framework.h"
#include "ProjectNative.h"

#include <stdlib.h>
#include <assert.h>
#include <Windows.h>


struct MyStruct
{
    PVOID pvFile;
    HANDLE hFileMap;
    HANDLE hFile;
    MyStruct2 mys2;
};


rkrs_ref rkrs_open_file(const char* pszPathName)
{
    HANDLE hFile = CreateFile(pszPathName, GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
    if (hFile == INVALID_HANDLE_VALUE)
        return NULL;

    DWORD dwFileSize = GetFileSize(hFile, NULL);
    HANDLE hFileMap = CreateFileMapping(hFile, NULL, PAGE_READONLY, 0, dwFileSize, NULL);
    if (hFileMap == NULL) {
        CloseHandle(hFile);
        return NULL;
    }

    void* pvFile = MapViewOfFile(hFileMap, FILE_MAP_READ, 0, 0, 0);
    if (pvFile == NULL) {
        CloseHandle(hFileMap);
        CloseHandle(hFile);
        return NULL;
    }

    MyStruct* mys = (MyStruct*)malloc(sizeof(MyStruct));
    assert(mys != NULL);

    *mys = { pvFile,hFileMap,hFile };
    return (rkrs_ref)mys;
}


void rkrs_close_file(rkrs_ref _this)
{
    assert(_this != NULL);
    MyStruct* mys = (MyStruct*)_this;

    UnmapViewOfFile(mys->pvFile);
    CloseHandle(mys->hFileMap);
    CloseHandle(mys->hFile);

    free(mys->mys2.ppbidd);
    free(mys);
}


void rkrs_parse(rkrs_ref _this, MyStruct2* mys2)
{
    assert(_this != NULL);
    assert(mys2 != NULL);
    MyStruct* mys = (MyStruct*)_this;

    void* pv = mys->pvFile;
    HEADER_H* h = (HEADER_H*)pv;
    RKRS_H* rkrs = (RKRS_H*)((char*)pv + 0x30);
    BID_H* bid = (BID_H*)((char*)pv + rkrs->offset);

    BIDD_H** ppbidd = (BIDD_H**)malloc(sizeof(void*) * rkrs->count);
    assert(ppbidd != NULL);

    for (size_t i = 0; i < rkrs->count; i++)
        ppbidd[i] = (BIDD_H*)((char*)pv + bid[i].offset);

    *mys2 = mys->mys2 = { h,rkrs,bid,ppbidd };
}


void rkrs_read_bidd_h(rkrs_ref _this, int idx, BIDD_H* pbidd)
{
    assert(_this != NULL);
    assert(pbidd != NULL);
    MyStruct* mys = (MyStruct*)_this;

    void* pv = mys->pvFile;
    RKRS_H* rkrs = (RKRS_H*)((char*)pv + 0x30);
    BID_H* bid = (BID_H*)((char*)pv + rkrs->offset);

    *pbidd = *(BIDD_H*)((char*)pv + bid[idx].offset);
}


void* rkrs_read_image_data(rkrs_ref _this, int idx)
{
    assert(_this != NULL);
    MyStruct* mys = (MyStruct*)_this;

    void* pv = mys->pvFile;
    RKRS_H* rkrs = (RKRS_H*)((char*)pv + 0x30);
    assert(idx < rkrs->count);

    BID_H* bid = (BID_H*)((char*)pv + rkrs->offset);
    BIDD_H* bidd = (BIDD_H*)((char*)pv + bid[idx].offset);

    BIDD_H _bidd = *bidd;
    int* data = (int*)bidd->data;
    int* bitmap = (int*)malloc(sizeof(int) * _bidd.width * _bidd.height);
    assert(bitmap != NULL);

    int* _bitmap = bitmap;

    if (_bidd.d4 == 1)
    {
        int v = 0;
        while (v = *(data++))
        {
            int val = v & 0x00ffffff;
            if (val != 0x00ff00ff)
                val |= 0xff000000;

            int len = v >> 24 & 0xff;
            if (len == 0xff)
                len += *(data++);

            for (int i = 0; i < len; i++)
                *(_bitmap++) = val;
        }
    }
    else if (_bidd.d4 == 0)
    {
        int pix = _bidd.width * _bidd.height;
        int curpix = 0;
        while (curpix++ < pix) {
            int val = *(data++);
            if (val != 0x00ff00ff)
                val |= 0xff000000;

            *(_bitmap++) = val;
        }
    }
    else if (_bidd.d4 == 5)
    {
        int alpha = 0;
        if (_bidd.d5 == 3)
            alpha = 0xff000000;

        int pix = _bidd.width * _bidd.height;
        int curpix = 0;
        while (curpix++ < pix)
            *(_bitmap++) = *(data++) | alpha;
    }
    return (void*)bitmap;
}


void rkrs_free_image_data(void* img)
{
    free(img);
}
