using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public static partial class ImplMethods
    {

        [ZZFunction("conio", "print")]
        public static ZZVoid Print(ZZString data)
        {
            Console.Write(data.ToString());

            return null;
        }

        [ZZFunction("conio", "println")]
        public static ZZVoid PrintLn(ZZString data)
        {
            Console.WriteLine(data.ToString());

            return null;
        }

        [ZZFunction("conio", "readln")]
        public static ZZString ReadLn()
        {
            return new ZZString(Console.ReadLine());
        }
    }
}
