using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainService
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RootFolderId { get; set; }
        public string PasswordSHA256 { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
