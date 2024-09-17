using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public static partial class ImplMethods
    {
        [ZZFunction("system", "Sys")]
        public static ZZInteger SystemFn(ZZString command)
        {
            return FastOps.System(command.Contents);
        }

        [ZZFunction("system", "Sleep")]
        public static ZZVoid Sleep(ZZInteger time)
        {
            if (time.Value < 0)
                throw new ArgumentException();

            System.Threading.Thread.Sleep((int)time.Value);

            return ZZVoid.Void;
        }

        [ZZFunction("system", "Env")]
        public static ZZObject GetEnv(ZZString name)
        {
            string env = null;

            try
            {
                env = Environment.GetEnvironmentVariable(name.Contents);

                if (env == null)
                    return ZZVoid.Void;
            }
            catch { return ZZVoid.Void; }

            return (ZZString)env;
        }

        [ZZFunction("system", "Ticks")]
        public static ZZInteger GetTickCount()
        {
            return Environment.TickCount64;
        }

        [ZZFunction("system", "Time")]
        public static ZZStruct GetDateAndTime()
        {
            DateTime now = DateTime.Now;

            ZZStruct strct = new ZZStruct();
            strct.Fields.Add("Day", new ZZInteger(now.Day));
            strct.Fields.Add("Month", new ZZInteger(now.Month));
            strct.Fields.Add("Year", new ZZInteger(now.Year));
            strct.Fields.Add("Time", new ZZFloat(now.TimeOfDay.TotalMilliseconds));
            strct.Fields.Add("Unix", new ZZInteger(((DateTimeOffset)now).ToUnixTimeMilliseconds()));

            return strct;
        }

        //[ZZFunction("system", "NativeFunction")]
        //public static ZZLongPointer NativeFunction(ZZString libName, ZZString fnName)
        //{
        //    return FastOps.GetNativeFunction(libName.Contents, fnName.Contents);
        //}

        //[ZZFunction("system", "NativeFunctionCall")]
        //public static ZZObject NativeFunctionCall(ZZLongPointer lp, ZZString callingConvention, 
        //    ZZArray argsArr)
        //{
        //    if (lp.Pointer == UIntPtr.Zero)
        //        throw new InterpreterRuntimeException("Tried to call a native function at a null pointer.");


        //}

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern UIntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
    }
}
