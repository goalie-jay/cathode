using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public static partial class ImplMethods
    {
        [ZZFunction("network", "ResolveHostname")]
        public static ZZObject ResolveHost(ZZString hostname)
        {
            string contents = hostname.Contents;

            try
            {
                return new ZZString(Dns.GetHostAddresses(contents)[0].ToString());
            }
            catch { return ZZVoid.Void; }
        }

        [ZZFunction("network", "Ping")]
        public static ZZInteger _Ping(ZZString addr)
        {
            try
            {
                Ping ping = new Ping();
                PingReply response = ping.Send(IPAddress.Parse(addr.Contents));

                return response.RoundtripTime;
            }
            catch { return -1; }
        }
    }
}
