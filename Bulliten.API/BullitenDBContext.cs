using Bulliten.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bulliten.API
{
    public class BullitenDBContext : DbContext
    {
        public BullitenDBContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<UserAccount> UserAccounts { get; set; }

        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Bulliten.db");
        }
    }
}
