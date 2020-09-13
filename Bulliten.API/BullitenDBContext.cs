﻿using Bulliten.API.Models;
using Microsoft.EntityFrameworkCore;

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

        public DbSet<FollowRecord> FollowerTable { get; set; }

        public DbSet<UserLike> UserLike { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FollowRecord>()
                .HasKey(fr => new { fr.FolloweeId, fr.FollowerId });

            modelBuilder.Entity<FollowRecord>()
                .HasOne(user => user.Followee)
                .WithMany(user => user.Followers)
                .HasForeignKey(user => user.FolloweeId);

            modelBuilder.Entity<UserLike>()
                .HasKey(ul => new { ul.UserId, ul.PostId });

            modelBuilder.Entity<UserLike>()
                .HasOne(userLike => userLike.Post)
                .WithMany(post => post.LikedBy);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite("Filename=Bulliten.db");
    }
}
