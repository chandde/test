using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainService
{
    public class File
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string ParentFolderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FileType { get; set; }
        public string SHA256 { get; set; }
    }
}
