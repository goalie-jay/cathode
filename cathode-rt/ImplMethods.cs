using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ZZFunction : Attribute
    {
        private string _zzname;
        public string InLanguageName
        {
            get
            {
                return _zzname;
            }
        }

        private string _namespace;
        public string Namespace
        {
            get
            {
                return _namespace;
            }
        }

        public ZZFunction(string ns, string zzname)
        {
            _namespace = ns;
            _zzname = zzname;
        }
    }

    public static partial class ImplMethods
    {
        
    }
}
