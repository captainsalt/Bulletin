using Bulliten.API.Models;
using Bulliten.API.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bulliten.API.Tests
{
    public class UserServiceTests
    {
        private readonly UserAccountService _target;
        private readonly UserAccount _testUser;
        private readonly BullitenDBContext _context;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<BullitenDBContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            _context = new BullitenDBContext(options);
            _target = new UserAccountService(_context, new AuthenticationService(null, _context));
            _testUser = new UserAccount { Username = "Test", Password = "Test" };
        }

        [Fact]
        public async Task Account_GetsCreated()
        {
            await _target.CreateAccount(_testUser);
            Assert.NotNull(_context.UserAccounts.ToList());
        }
    }
}
