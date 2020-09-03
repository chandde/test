using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPageServer
{
    public class Cache
    {
        public string CommonVersion { get; set; }
        public string IndexHtml { get; set; }
        public Dictionary<string /*page*/, string /*version*/> PageVersion { get; set; }
    }
}
