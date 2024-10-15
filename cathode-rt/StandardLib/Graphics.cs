using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public static partial class ImplMethods
    {
        [ZZFunction("graphics", "HideConsole")]
        public static ZZVoid HideConsole()
        {
            FastOps.FreeConsole();
            return ZZVoid.Void;
        }

        [ZZFunction("graphics", "ShowConsole")]
        public static ZZVoid ShowConsole()
        {
            FastOps.AllocConsole();
            return ZZVoid.Void;
        }
    }
}
