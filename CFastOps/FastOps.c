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

EXPORT VOID ExecuteNativeFunction(LONGLONG nAddr, LPVOID* lpArgBytesArr, INT* iArgumentTypesArray, INT iArgc, INT iReturnType, LPVOID lpReturnData)
{
    // I don't even know how to cook this. Here's my best guess
}

EXPORT INT System(LPCSTR sz)
{
    return system(sz);
}

EXPORT VOID Setup()
{
	
}