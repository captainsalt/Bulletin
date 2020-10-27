using Bulletin.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulletin.API
{
    public class BulletinDBContext : DbContext
    {
        public BulletinDBContext()
        {
            Database.EnsureCreated();
        }

        public BulletinDBContext(DbContextOptions optionsBuilder)
            : base(optionsBuilder)
        {
            Database.EnsureCreated();
        }

        public DbSet<UserAccount> UserAccounts { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<FollowRecord> FollowerTable { get; set; }

        public DbSet<UserLike> UserLike { get; set; }

        public DbSet<UserRepost> UserReposts { get; set; }

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

            modelBuilder.Entity<UserRepost>()
                .HasKey(ur => new { ur.UserId, ur.PostId });
        }
    }
}
