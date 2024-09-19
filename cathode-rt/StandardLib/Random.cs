using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public static partial class ImplMethods
    {
        [ZZFunction("core", "rSeed")]
        public static ZZVoid SetSeed(ZZInteger seed)
        {
            FastOps.SetRandomSeed(seed.Value);
            return ZZVoid.Void;
        }

        [ZZFunction("core", "RandomInt")]
        public static ZZInteger RandomInt()
        {
            return RandomNumberGen.GetRandomInt();
        }

        [ZZFunction("core", "RandomFloat")]
        public static ZZFloat RandomFloat()
        {
            return RandomNumberGen.GetRandomFloat();
        }

        [ZZFunction("core", "RandomBytes")]
        public static ZZArray RandomBytes(ZZInteger length)
        {
            if (length.Value < 0)
                throw new ArgumentException();

            byte[] bytes = RandomNumberGen.GetRandomBytes((int)length.Value);

            List<ZZObject> arrContents = new List<ZZObject>();
            foreach (byte b in bytes)
                arrContents.Add(new ZZByte(b));

            return new ZZArray(arrContents.ToArray());
        }
    }
}
