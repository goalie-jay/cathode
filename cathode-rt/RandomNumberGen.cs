using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public static class RandomNumberGen
    {
        private static Random random = new Random(Environment.TickCount);

        public static ZZInteger GetRandomInt()
        {
            // Ints are actually longs
            byte[] longBytes = GetRandomBytes(sizeof(long));
            return (ZZInteger)BitConverter.ToInt64(longBytes);
        }

        public static ZZFloat GetRandomFloat()
        {
            // Floats are actually doubles
            byte[] longBytes = GetRandomBytes(sizeof(double));
            return (ZZFloat)BitConverter.ToDouble(longBytes);
        }

        public static byte[] GetRandomBytes(int count)
        {
            byte[] data = new byte[count];
            random.NextBytes(data);

            return data;
        }
    }
}
