﻿using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using System.Threading.Tasks;

namespace Bulliten.API.Services
{
    public interface IUserAccountService
    {
        UserAccount GetUserByUsername(string username);

        Task FollowUser(UserAccount ctxUser, string username);

        Task UnfollowUser(UserAccount ctxUser, string username);

        Task<AuthenticationResponse> CreateAccount(UserAccount account);

        Task<AuthenticationResponse> Login(UserAccount account);
    }
}
