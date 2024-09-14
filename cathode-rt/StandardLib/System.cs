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
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = Environment.GetEnvironmentVariable("comspec");
            info.Arguments = "/c " + "\"" + command.ToString() + "\"";

            using (Process proc = Process.Start(info))
            {
                proc.WaitForExit();
                return new ZZInteger(proc.ExitCode);
            }
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

        //[ZZFunction("system", "importnative")]
        //public static ZZLongPointer ImportNative(string libName)
        //{
        //    return new ZZLongPointer(LoadLibrary(libName));
        //}

        //[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        //static extern UIntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
    }
}
