using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bulliten.API.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly BullitenDBContext _context;
        private readonly IAuthenticationService _authService;

        public UserAccountService(BullitenDBContext context, IAuthenticationService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<AuthenticationResponse> CreateAccount(UserAccount formAccount)
        {
            bool isInvalidUsername = await _context.UserAccounts.AnyAsync(u => u.Username == formAccount.Username);

            if (isInvalidUsername)
                throw new ArgumentException($"Username \"{formAccount.Username}\" is already in use");

            await _context.UserAccounts.AddAsync(formAccount);
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
                .SingleOrDefaultAsync(u => u.Username == formAccount.Username && u.Password == formAccount.Password);

            if (matchedAccount == null)
                throw new ArgumentException("Invalid username or password");

            return await _authService
                .Authenticate(new AuthenticationRequest
                {
                    Username = matchedAccount.Username,
                    Password = matchedAccount.Password
                });
        }

        /// <summary>
        /// Adds <paramref name="ctxUser"/> to <paramref name="username"/>'s Followers list
        /// </summary>
        /// <param name="ctxUser">User making the request to follow</param>
        /// <param name="username">Username of account being followed</param>
        /// <returns></returns>
        public async Task FollowUser(UserAccount ctxUser, string username)
        {
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
        public async Task UnfollowUser(UserAccount ctxUser, string username)
        {
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

        public UserAccount GetUserByUsername(string username) => throw new NotImplementedException();
    }
}
