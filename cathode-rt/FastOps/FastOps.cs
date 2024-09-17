using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

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

        public static ZZLongPointer GetNativeFunction(string lib, string fn)
        {
            return new ZZLongPointer((nuint)_FindFunctionAndLoadLibraryIfNecessary(lib, fn));
        }

        [DllImport("CFastOps.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "LongToString")]
        private static extern unsafe void _LongToString(long* ptr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder result);

        [DllImport("CFastOps.dll", EntryPoint = "FastIncByOne")]
        private static extern unsafe void _FastIncByOne(long* ptr);
        
        [DllImport("CFastOps.dll")]
        public static extern int System([MarshalAs(UnmanagedType.LPStr)] string s);

        [DllImport("CFastOps.dll", EntryPoint = "FindFunctionAndLoadLibraryIfNecessary")]
        private static extern long _FindFunctionAndLoadLibraryIfNecessary([MarshalAs(UnmanagedType.LPStr)] 
            string libName, [MarshalAs(UnmanagedType.LPStr)] string procName);

        [DllImport("CFastOps.dll")]
        public static extern void Setup();
    }
}
