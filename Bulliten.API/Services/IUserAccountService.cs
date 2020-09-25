using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using System.Threading.Tasks;

namespace Bulliten.API.Services
{
    public interface IUserAccountService
    {
        Task<AuthenticationResponse> CreateAccount(UserAccount formAccount);

        Task FollowUser(UserAccount ctxUser, string username);

        Task<(int following, int followers)> GetFollowInfo(string username);

        Task<UserAccount> GetUserByUsername(string username);

        Task<AuthenticationResponse> Login(UserAccount formAccount);

        Task UnfollowUser(UserAccount ctxUser, string username);

        Task<bool> UserIsFollowing(UserAccount ctxUser, string followeeUsername);
    }
}
