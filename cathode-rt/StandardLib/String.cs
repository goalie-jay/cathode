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

        [ZZFunction("string", "sSplit")]
        public static ZZArray StringSplit(ZZString str, ZZString delim)
        {
            string[] split = str.Contents.Split(new string[] { delim.Contents }, StringSplitOptions.None);

            List<ZZString> langStrings = new List<ZZString>();
            foreach (string s in split)
                langStrings.Add(new ZZString(s));

            return new ZZArray(langStrings.ToArray());
        }

        [ZZFunction("string", "sEmpty")]
        public static ZZInteger StringEmpty(ZZString str)
        {
            if (str.Contents.Length == 0)
                return 1;

            return 0;
        }

        [ZZFunction("string", "sTrim")]
        public static ZZString StringTrim(ZZString str)
        {
            return str.Contents.Trim();
        }
    }
}
