using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public static partial class ImplMethods
    {
        [ZZFunction("conio", "cWidth")]
        public static ZZInteger GetConsoleWidth()
        {
            return Console.BufferWidth;
        }

        [ZZFunction("conio", "cHeight")]
        public static ZZInteger GetConsoleHeight()
        {
            return Console.BufferHeight;
        }

        [ZZFunction("conio", "Title")]
        public static ZZVoid SetConsoleTitle(ZZString str)
        {
            Console.Title = str.Contents;
            return ZZVoid.Void;
        }

        [ZZFunction("conio", "Print")]
        public static ZZVoid Print(ZZObject obj)
        {
            Console.Write(obj.ToInLanguageString().ToString());
            return ZZVoid.Void;
        }

        [ZZFunction("conio", "PrintLn")]
        public static ZZVoid PrintLn(ZZObject obj)
        {
            Console.WriteLine(obj.ToInLanguageString().ToString());
            return ZZVoid.Void;
        }

        [ZZFunction("conio", "ReadLn")]
        public static ZZString ReadLn()
        {
            return new ZZString(Console.ReadLine());
        }
    }
}
