using Bulliten.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bulliten.API.Services
{
    interface IUserAccountService
    {
        UserAccount GetUserByUsername(string username);

        Task FollowUser(string username);

        Task UnfollowUser(string username);

        Task CreateAccount(string username, string password);

        Task Login(string username, string password);
    }
}
