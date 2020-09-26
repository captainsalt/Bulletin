using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using System.Threading.Tasks;

namespace Bulliten.API.Services
{
    public interface IUserAccountService
    {
        Task<AuthenticationResponse> CreateAccount(UserAccount formAccount);

        Task FollowUser(string username);

        Task<(int followingCount, int followerCount)> GetFollowInfo(string username);

        Task<UserAccount> GetUserByUsername(string username);

        Task<AuthenticationResponse> Login(UserAccount formAccount);

        Task UnfollowUser(string username);

        Task<bool> UserIsFollowing(string followeeUsername);
    }
}
