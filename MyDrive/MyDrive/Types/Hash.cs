using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainService
{
    public class Hash
    {
        public string Id { get; set; }
        public string SHA256 { get; set; }
        public string URL { get; set; }
    }
}
