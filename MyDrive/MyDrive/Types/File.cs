using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainService
{
    public class File
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string ParentFolderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FileType { get; set; }
        // workaround with mysql which does not support array datatype
        public string Content { get; set; }
    }
}
