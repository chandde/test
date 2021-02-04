using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainService.Types
{
    public class ClientContext
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FolderId { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
    }
}
