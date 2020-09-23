using Bulliten.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Bulliten.API
{
    public interface IBullitenDBContext 
    {
        DbSet<FollowRecord> FollowerTable { get; set; }

        DbSet<Post> Posts { get; set; }

        DbSet<UserAccount> UserAccounts { get; set; }

        DbSet<UserLike> UserLike { get; set; }

        DbSet<UserRepost> UserReposts { get; set; }

        Task<int> SaveChangesAsync();
    }
}