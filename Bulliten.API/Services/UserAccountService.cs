using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using Microsoft.EntityFrameworkCore;
using System;
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

        public Task FollowUser(string username) => throw new NotImplementedException();
        public UserAccount GetUserByUsername(string username) => throw new NotImplementedException();
        public Task UnfollowUser(string username) => throw new NotImplementedException();
    }
}
