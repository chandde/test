using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPageServer
{
    public class AppConfiguration
    {
        public string Cdn { get; set; }
        public string Blob { get; set; }
        public string WwwDomain { get; set; }
        public string ShortDomain { get; set; }
        public List<string> AppServiceDomains { get; set; }
    }
}
