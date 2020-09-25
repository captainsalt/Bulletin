using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using Bulliten.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bulliten.API.Tests
{
    public class UserServiceTests : IDisposable
    {
        private readonly UserAccountService _target;
        private readonly UserAccount _testUser;
        private readonly BullitenDBContext _context;

        public UserServiceTests()
        {
            _context = new ConnectionFactory().CreateContextForSQLite();

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m["Secret"]).Returns("SecretTestSTring");

            _target = new UserAccountService(_context, new AuthenticationService(configMock.Object, _context));
            _testUser = new UserAccount { Username = "Test", Password = "Test" };
        }

        [Fact]
        public async Task CreateAccount_AddsUserToDababase()
        {
            await _target.CreateAccount(_testUser);

            Assert.NotNull(_context.UserAccounts.ToList());
        }

        [Fact]
        public async Task CreateAccount_Throws_IfAccountUsernameExists()
        {
            await _target.CreateAccount(_testUser);

            await Assert.ThrowsAsync<ArgumentException>(async () => await _target.CreateAccount(_testUser));
        }

        [Fact]
        public async Task Login_ReturnsCredentials()
        {
            await _target.CreateAccount(_testUser);
            AuthenticationResponse response = await _target.Login(_testUser);

            Assert.NotNull(response.User);
            Assert.NotNull(response.Token);
        }

        [Fact]
        public async Task Login_Throws_IfInvalidCredentials()
        {
            await _target.CreateAccount(_testUser);
            _testUser.Password = "WrongPassword";

            await Assert.ThrowsAsync<ArgumentException>(async () => await _target.Login(_testUser));
        }

        public void Dispose() => _context.Dispose();
    }
}
