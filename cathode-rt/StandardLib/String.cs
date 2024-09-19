using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public static partial class ImplMethods
    {
        [ZZFunction("string", "sIdxOf")]
        public static ZZInteger StringIndexOf(ZZString str, ZZString substr)
        {
            try
            {
                return (ZZInteger)str.Contents.IndexOf(substr.Contents);
            }
            catch { return -1; }
        }
    }
}
