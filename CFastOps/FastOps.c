#include "FastOps.h"
#include <stdio.h>

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
    for (INT i = 0; i < count; ++i)
        ((LPBYTE)lp)[i] = (BYTE)rand();
}

EXPORT INT System(LPCSTR sz)
{
    return system(sz);
}

EXPORT VOID SetRandomSeed(LONGLONG seed)
{
    srand(seed);
}

EXPORT LONGLONG AttemptToEnterDebugPrivilege()
{
    HANDLE hTok;
    OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY | TOKEN_ADJUST_PRIVILEGES, &hTok);
    LUID c;
    LookupPrivilegeValueA(NULL, SE_DEBUG_NAME, &c);
    
    TOKEN_PRIVILEGES tkp;
    tkp.PrivilegeCount = 1;
    tkp.Privileges[0].Luid = c;
    tkp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

    if (AdjustTokenPrivileges(hTok, FALSE, &tkp, sizeof(TOKEN_PRIVILEGES), NULL, NULL))
        return 1;

    return 0;
}

EXPORT VOID AttemptToCloseProcess(ULONGLONG hHandle)
{
    CloseHandle(hHandle);
}

EXPORT CHAR AttemptToReadProcess(ULONGLONG hHandle, ULONGLONG nAddr, ULONGLONG nBufferLen, LPVOID lpBuf)
{
    SIZE_T dw;
    if (ReadProcessMemory(hHandle, nAddr, lpBuf, nBufferLen, &dw))
        return 1;

    return 0;
}

EXPORT CHAR AttemptToWriteProcess(ULONGLONG hHandle, ULONGLONG nAddr, ULONGLONG nBufferLen, LPVOID lpBuf)
{
    SIZE_T dw;
    if (WriteProcessMemory(hHandle, nAddr, lpBuf, nBufferLen, &dw))
        return 1;

    return 0;
}

EXPORT ULONGLONG AttemptToOpenProcess(DWORD dwProcessId, CHAR cRead, CHAR cWrite)
{
    const char* pid[MAX_PATH];
    sprintf_s(pid, MAX_PATH, "pid: %d", dwProcessId);
    MessageBoxA(NULL, pid, "", MB_OK);

    DWORD dwAccessConstant = PROCESS_QUERY_INFORMATION;

    if (cRead)
        dwAccessConstant = dwAccessConstant | PROCESS_VM_READ;

    if (cWrite)
        dwAccessConstant = dwAccessConstant | PROCESS_VM_WRITE;

    HANDLE hProcess = OpenProcess(dwAccessConstant, FALSE, dwProcessId);

    if (!hProcess)
        return 0;

    return (ULONGLONG)hProcess;
}

EXPORT VOID Setup()
{
    srand(GetTickCount());
}