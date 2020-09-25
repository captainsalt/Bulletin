using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using Bulliten.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bulliten.API.Tests
{
    public class UserServiceTests : IDisposable
    {
        private readonly UserAccountService _target;
        private readonly List<UserAccount> _testAccounts;
        private readonly BullitenDBContext _context;

        public UserServiceTests()
        {
            _context = new ConnectionFactory().CreateContextForSQLite();

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m["Secret"]).Returns("SecretTestSTring");

            _testAccounts = new List<UserAccount>
            {
                new UserAccount { Username = "user1", Password = "Test" },
                new UserAccount { Username = "user2", Password = "Test" }
            };

            _target = new UserAccountService(_context, new AuthenticationService(configMock.Object, _context));
        }

        #region CreateAccount
        [Fact]
        public async Task CreateAccount_AddsUserToDababase()
        {
            await _target.CreateAccount(_testAccounts[0]);

            Assert.NotNull(_context.UserAccounts.ToList());
        }

        [Fact]
        public async Task CreateAccount_Throws_IfAccountUsernameExists()
        {
            await _target.CreateAccount(_testAccounts[0]);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.CreateAccount(_testAccounts[0])
            );
        }
        #endregion

        #region Login
        [Fact]
        public async Task Login_ReturnsCredentials()
        {
            await _target.CreateAccount(_testAccounts[0]);
            AuthenticationResponse response = await _target.Login(_testAccounts[0]);

            Assert.NotNull(response.User);
            Assert.NotNull(response.Token);
        }

        [Fact]
        public async Task Login_Throws_IfInvalidCredentials()
        {
            await _target.CreateAccount(_testAccounts[0]);
            _testAccounts[0].Password = "WrongPassword";

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.Login(_testAccounts[0])
            );
        }
        #endregion

        #region FollowUser
        [Fact]
        public async Task FollowUser_AddsFollowRecordToDatabase()
        {
            AuthenticationResponse auth1 = await _target.CreateAccount(_testAccounts[0]);
            AuthenticationResponse auth2 = await _target.CreateAccount(_testAccounts[1]);

            UserAccount user1 = _context.UserAccounts.Find(auth1.User.ID);
            UserAccount user2 = _context.UserAccounts.Find(auth2.User.ID);

            await _target.FollowUser(auth1.User, user2.Username);

            FollowRecord followRecord = _context.FollowerTable.First();

            Assert.Equal(user2.Username, followRecord.Followee.Username);
            Assert.Equal(user1.Username, followRecord.Follower.Username);
        }

        [Fact]
        public async Task FollowUser_Throws_IfRequestToFollowSelf()
        {
            AuthenticationResponse auth = await _target.CreateAccount(_testAccounts[0]);
            UserAccount user = _context.UserAccounts.Find(auth.User.ID);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _target.FollowUser(user, user.Username)
            );
        }

        [Fact]
        public async Task FollowUser_Throws_IfUserDoesNotExist()
        {
            AuthenticationResponse auth = await _target.CreateAccount(_testAccounts[0]);
            UserAccount user = _context.UserAccounts.Find(auth.User.ID);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.FollowUser(user, "NonExistingUsername")
            );
        }

        [Fact]
        public async Task FollowUser_Throws_IfAlreadyFollowing()
        {
            AuthenticationResponse auth1 = await _target.CreateAccount(_testAccounts[0]);
            AuthenticationResponse auth2 = await _target.CreateAccount(_testAccounts[1]);

            UserAccount user1 = _context.UserAccounts.Find(auth1.User.ID);
            UserAccount user2 = _context.UserAccounts.Find(auth2.User.ID);

            await _target.FollowUser(auth1.User, user2.Username);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.FollowUser(auth1.User, user2.Username)
            );
        }
        #endregion

        public void Dispose() => _context.Dispose();
    }
}
