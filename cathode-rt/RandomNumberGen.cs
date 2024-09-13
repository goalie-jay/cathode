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

        public static byte[] GetRandomBytes(int count)
        {
            byte[] data = new byte[count];
            random.NextBytes(data);

            return data;
        }
    }
}
