#pragma once
#include <Windows.h>

#define EXPORT __declspec(dllexport)

extern LPCSTR L2SWorkingMemory;

EXPORT VOID FastIncByOne(PLONGLONG pLong);
EXPORT VOID FastInc(PLONGLONG pLong, LONGLONG nAmount);
EXPORT VOID LongToString(PLONGLONG, LPCSTR*);
EXPORT LONGLONG FindFunctionAndLoadLibraryIfNecessary(LPCSTR, LPCSTR);
EXPORT VOID ExecuteNativeFunction(LONGLONG nAddr, LPVOID* lpArgBytesArr, INT* iArgumentTypesArray, INT iArgc, INT iReturnType, LPVOID lpReturnData);
EXPORT INT System(LPCSTR);

EXPORT VOID Setup();