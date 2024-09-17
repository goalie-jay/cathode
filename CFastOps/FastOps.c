#include "FastOps.h"

EXPORT VOID FastIncByOne(PLONGLONG pLong)
{
	++(*pLong);
}

EXPORT VOID FastInc(PLONGLONG pLong, LONGLONG nAmount)
{
	*pLong += nAmount;
}

EXPORT VOID LongToString(PLONGLONG pLong, LPCSTR/***/ pOutput)
{
    CHAR sz[30];
    
    _ltoa_s(*pLong, sz, 30, 10);

    // *pOutput = (LPCSTR)malloc(31);
    //strcpy_s(/***/pOutput, 31, sz);

    memcpy(pOutput, sz, 31);
}

EXPORT LONGLONG FindFunctionAndLoadLibraryIfNecessary(LPCSTR szLibName, LPCSTR szProcName)
{
    HANDLE hModule;
    hModule = GetModuleHandleA(szLibName);

    if (!hModule)
        hModule = LoadLibraryA(szLibName);

    if (!hModule)
        return NULL;

    return GetProcAddress(hModule, szProcName);
}

EXPORT VOID GetRandomBytes(LPVOID lp, LONGLONG count)
{
    static INT seeded = 0;

    if (seeded == 0)
    {
        seeded = 1;
        srand(GetTickCount());
    }

    for (INT i = 0; i < count; ++i)
        ((LPBYTE)lp)[i] = (BYTE)rand();
}

EXPORT INT System(LPCSTR sz)
{
    return system(sz);
}

EXPORT VOID Setup()
{
	
}