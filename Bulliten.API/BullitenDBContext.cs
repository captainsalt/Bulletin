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

        public DbSet<UserXUser> FollowerTable { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserXUser>()
                .HasKey(fr => new { fr.FolloweeId, fr.FollowerId });

            modelBuilder.Entity<UserXUser>()
                .HasOne(user => user.Followee)
                .WithMany(user => user.Followers)
                .HasForeignKey(user => user.FolloweeId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Bulliten.db");
        }
    }
}
