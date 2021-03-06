﻿using Bulletin.API.Middleware;
using Bulletin.API.Models;
using Bulletin.API.Models.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bulletin.API.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly BulletinDBContext _context;
        private readonly IAuthenticationService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAccountService(
            BulletinDBContext context,
            IAuthenticationService authService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthenticationResponse> CreateAccount(UserAccount formAccount)
        {
            bool isInvalidUsername = await _context.UserAccounts.AnyAsync(u => u.Username == formAccount.Username);

            if (isInvalidUsername)
                throw new ArgumentException($"Username \"{formAccount.Username}\" is already in use");

            var dbUserAccount = new UserAccount() { Username = formAccount.Username, Password = formAccount.Password };
            dbUserAccount.Password = await _authService.HashPassword(dbUserAccount.Password);

            await _context.UserAccounts.AddAsync(dbUserAccount);
            await _context.SaveChangesAsync();

            return await _authService
                .Authenticate(new AuthenticationRequest
                {
                    Username = formAccount.Username,
                    Password = formAccount.Password
                });
        }

        public async Task<AuthenticationResponse> Login(UserAccount formAccount)
        {
            UserAccount matchedAccount = await _context.UserAccounts
                .SingleOrDefaultAsync(u => u.Username == formAccount.Username);

            bool isCorrectPassword = await _authService.CheckPassword(matchedAccount, formAccount.Password);

            if (matchedAccount == null || isCorrectPassword == false)
                throw new ArgumentException("Invalid username or password");

            return await _authService
                .Authenticate(new AuthenticationRequest
                {
                    Username = formAccount.Username,
                    Password = formAccount.Password
                });
        }

        /// <summary>
        /// Adds <paramref name="ctxUser"/> to <paramref name="username"/>'s Followers list
        /// </summary>
        /// <param name="ctxUser">User making the request to follow</param>
        /// <param name="username">Username of account being followed</param>
        /// <returns></returns>
        public async Task FollowUser(string username)
        {
            UserAccount ctxUser = GetContextUser();

            if (ctxUser.Username == username)
                throw new ArgumentException("Cannot follow yourself");

            UserAccount targetUser = await _context.UserAccounts
                .Include(u => u.Followers)
                .SingleOrDefaultAsync(u => u.Username == username);

            if (targetUser == null)
                throw new ArgumentException("User does not exist");

            FollowRecord followRecord = targetUser.Followers
                .SingleOrDefault(fr => fr.FollowerId == ctxUser.ID);

            if (followRecord != null)
                throw new ArgumentException("Already following");

            targetUser.Followers.Add(new FollowRecord
            {
                Followee = targetUser,
                Follower = ctxUser,
            });

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes <paramref name="ctxUser"/> from <paramref name="username"/>'s Followers list
        /// </summary>
        /// <param name="ctxUser">User making the request to unfollow</param>
        /// <param name="username">Username of account being unfollowed</param>
        public async Task UnfollowUser(string username)
        {
            UserAccount ctxUser = GetContextUser();

            if (ctxUser.Username == username)
                throw new ArgumentException("Cannot unfollow yourself");

            UserAccount targetUser = await _context.UserAccounts
                .Include(u => u.Followers)
                .SingleOrDefaultAsync(u => u.Username == username);

            if (targetUser == null)
                throw new ArgumentException("User does not exist");

            FollowRecord followRecord = targetUser.Followers
                .SingleOrDefault(fr => fr.FollowerId == ctxUser.ID);

            if (followRecord == null)
                throw new ArgumentException("Cannot unfollow someone you're not following");

            targetUser.Followers.Remove(followRecord);
            await _context.SaveChangesAsync();
        }

        public async Task<UserAccount> GetUserByUsername(string username)
        {
            UserAccount user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                throw new ArgumentException("User does not exist");

            return user;
        }

        public async Task<(int followingCount, int followerCount)> GetFollowInfo(string username)
        {
            UserAccount user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Username == username);

            int followerCount = await _context.FollowerTable
                .Where(fr => fr.FolloweeId == user.ID)
                .CountAsync();

            int followingCount = await _context.FollowerTable
                .Where(fr => fr.FollowerId == user.ID)
                .CountAsync();

            return (followingCount, followerCount);
        }

        public async Task<bool> UserIsFollowing(string followeeUsername)
        {
            await _context.FollowerTable.LoadAsync();
            UserAccount followee = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Username == followeeUsername);
            return followee.Followers.Any(u => u.FollowerId == GetContextUser().ID);
        }

        private UserAccount GetContextUser() =>
            (UserAccount)_httpContextAccessor.HttpContext.Items[JwtMiddleware.CONTEXT_USER];
    }
}
