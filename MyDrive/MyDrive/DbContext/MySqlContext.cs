using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace MainService
{
    public class MySqlContext : DbContext
    {
        public MySqlContext(DbContextOptions<MySqlContext> options)
            : base(options)
        {
        }

        public DbSet<File> File { get; set; }

        public DbSet<User> User { get; set; }

        //public DbSet<Folder> Folder { get; set; }

        public DbSet<Hash> Hash { get; set; }

        public DbSet<HashMap> HashMap { get; set; }
    }
}
