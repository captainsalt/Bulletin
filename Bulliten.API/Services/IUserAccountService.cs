using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bulliten.API.Services
{
    public interface IUserAccountService
    {
        UserAccount GetUserByUsername(string username);

        Task FollowUser(string username);

        Task UnfollowUser(string username);

        Task<AuthenticationResponse> CreateAccount(UserAccount account);

        Task Login(string username, string password);
    }
}
