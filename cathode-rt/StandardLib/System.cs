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
        [ZZFunction("system", "sys")]
        public static ZZInteger SystemFn(ZZString data)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = Environment.GetEnvironmentVariable("comspec");
            info.Arguments = "/c " + "\"" + data.ToString() + "\"";

            using (Process proc = Process.Start(info))
            {
                proc.WaitForExit();
                return new ZZInteger(proc.ExitCode);
            }
        }

        [ZZFunction("system", "sleep")]
        public static ZZVoid Sleep(ZZInteger time)
        {
            System.Threading.Thread.Sleep((int)time.Value);

            return new ZZVoid();
        }

        [ZZFunction("system", "importnative")]
        public static ZZLongPointer ImportNative(string libName)
        {
            return new ZZLongPointer(LoadLibrary(libName));
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern UIntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
    }
}
