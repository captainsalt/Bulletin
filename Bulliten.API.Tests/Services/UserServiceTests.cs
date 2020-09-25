using Bulliten.API.Models;
using Bulliten.API.Services;
using Microsoft.EntityFrameworkCore;
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
        public async Task Account_GetsCreated()
        {
            await _target.CreateAccount(_testUser);
            Assert.NotNull(_context.UserAccounts.ToList());
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
