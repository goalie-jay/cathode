using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public static partial class ImplMethods
    {
        [ZZFunction("conio", "Title")]
        public static ZZVoid SetConsoleTitle(ZZString str)
        {
            Console.Title = str.Contents;
            return ZZVoid.Void;
        }

        [ZZFunction("conio", "Print")]
        public static ZZVoid Print(ZZString str)
        {
            Console.Write(str.ToString());
            return ZZVoid.Void;
        }

        [ZZFunction("conio", "PrintLn")]
        public static ZZVoid PrintLn(ZZString str)
        {
            Console.WriteLine(str.ToString());
            return ZZVoid.Void;
        }

        [ZZFunction("conio", "ReadLn")]
        public static ZZString ReadLn()
        {
            return new ZZString(Console.ReadLine());
        }
    }
}
