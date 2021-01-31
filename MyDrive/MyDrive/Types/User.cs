using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainService
{
    public class User
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RootFolderId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
