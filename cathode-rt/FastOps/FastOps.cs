using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace cathode_rt
{
    public static class FastOps
    {
        public static void FastInc(ZZInteger i)
        {
            unsafe
            {
                fixed (long* lp = &i.Value)
                    _FastIncByOne(lp);
            }
        }

        public static string Long2String(long l)
        {
            unsafe
            {
                StringBuilder sb = new StringBuilder(31);
                _LongToString(&l, sb);
                return sb.ToString();
            }
        }

        //public static ZZLongPointer GetNativeFunction(string lib, string fn)
        //{
        //    return new ZZLongPointer((nuint)_FindFunctionAndLoadLibraryIfNecessary(lib, fn));
        //}

        public static unsafe byte[] AttemptToReadProcess(UIntPtr handle, UIntPtr baseAddr, ulong count)
        {
            byte[] arr = new byte[count];
            bool retVal = false;

            retVal = ReadProcessMemory(handle, baseAddr, arr, (int)count, out _);

            if (!retVal)
                return null;

            return arr;
        }

        public static unsafe byte AttemptToWriteProcess(UIntPtr handle, UIntPtr baseAddr, byte[] data)
        {
            fixed (byte* lp = data)
            {
                if (WriteProcessMemory(handle, baseAddr, data, data.Length, out _))
                    return 1;

                return 0;
            }
        }

        [DllImport("CFastOps.dll")]
        public static extern long AttemptToEnterDebugPrivilege();

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(UIntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(UIntPtr hProcess, UIntPtr lpBaseAddress, [Out] byte[] lpBuffer,
            int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(UIntPtr hProcess, UIntPtr lpBaseAddress, byte[] lpBuffer, 
            int nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UIntPtr OpenProcess(uint processAccess, bool bInheritHandle, uint processId);

        [DllImport("CFastOps.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "LongToString")]
        private static extern unsafe void _LongToString(long* ptr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder result);

        [DllImport("CFastOps.dll", EntryPoint = "FastIncByOne")]
        private static extern unsafe void _FastIncByOne(long* ptr);
        
        [DllImport("CFastOps.dll")]
        public static extern int System([MarshalAs(UnmanagedType.LPStr)] string s);

        //[DllImport("CFastOps.dll", EntryPoint = "FindFunctionAndLoadLibraryIfNecessary")]
        //private static extern long _FindFunctionAndLoadLibraryIfNecessary([MarshalAs(UnmanagedType.LPStr)] 
        //    string libName, [MarshalAs(UnmanagedType.LPStr)] string procName);

        [DllImport("CFastOps.dll")]
        public static unsafe extern void GetRandomBytes(byte* ptr, long count);
        [DllImport("CFastOps.dll")]
        public static extern void SetRandomSeed(long seed);

        [DllImport("CFastOps.dll")]
        public static extern void Setup();
    }
}
