using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using Bulliten.API.Services;
using Bulliten.API.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Bulliten.API.Tests.Helpers.BullitenDBContextExtensions;

namespace Bulliten.API.Tests.Services
{
    public class UserServiceTests : IDisposable
    {
        private readonly UserAccountService _target;
        private readonly BullitenDBContext _context;

        public UserServiceTests()
        {
            _context = new ConnectionFactory().CreateContextForSQLite();

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m["Secret"]).Returns("SecretTestSTring");

            _target = new UserAccountService(_context, new AuthenticationService(configMock.Object, _context));
        }

        #region CreateAccount
        [Fact]
        public async Task CreateAccount_AddsUserToDababase()
        {
            UserAccount testAccount = GenerateUserAccounts(1).First();
            await _target.CreateAccount(testAccount);

            Assert.NotNull(_context.UserAccounts.ToList());
        }

        [Fact]
        public async Task CreateAccount_Throws_IfAccountUsernameExists()
        {
            UserAccount testAccount = GenerateUserAccounts(1).First();
            _context.Setup(context =>
            {
                context.UserAccounts.Add(testAccount);
            });

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.CreateAccount(testAccount)
            );
        }
        #endregion

        #region Login
        [Fact]
        public async Task Login_ReturnsCredentials()
        {
            UserAccount testAccount = GenerateUserAccounts(1).First();
            _context.Setup(context =>
            {
                context.UserAccounts.Add(testAccount);
            });

            AuthenticationResponse response = await _target.Login(testAccount);

            Assert.NotNull(response.User);
            Assert.NotNull(response.Token);
        }

        [Fact]
        public async Task Login_Throws_IfInvalidCredentials()
        {
            UserAccount testAccount = GenerateUserAccounts(1).First();
            _context.Setup(context =>
            {
                context.UserAccounts.Add(testAccount);
            });

            testAccount.Password = "WrongPassword";

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.Login(testAccount)
            );
        }
        #endregion

        #region FollowUser
        [Fact]
        public async Task FollowUser_AddsFollowRecordToDatabase()
        {
            IEnumerable<UserAccount> users = GenerateUserAccounts(2);
            _context.Setup(context =>
            {
                context.UserAccounts.AddRange(users);
            });

            UserAccount user1 = _context.UserAccounts.Find(users.First().ID);
            UserAccount user2 = _context.UserAccounts.Find(users.Last().ID);

            await _target.FollowUser(user1, user2.Username);

            FollowRecord followRecord = _context.FollowerTable.First();

            Assert.Equal(user2.Username, followRecord.Followee.Username);
            Assert.Equal(user1.Username, followRecord.Follower.Username);
        }

        [Fact]
        public async Task FollowUser_Throws_IfRequestToFollowSelf()
        {
            UserAccount testUser = GenerateUserAccounts(1).First();
            _context.Setup(context =>
            {
                context.UserAccounts.Add(testUser);
            });

            UserAccount user = _context.UserAccounts.Find(testUser.ID);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _target.FollowUser(user, user.Username)
            );
        }

        [Fact]
        public async Task FollowUser_Throws_IfUserDoesNotExist()
        {
            UserAccount testUser = GenerateUserAccounts(1).First();
            _context.Setup(context =>
            {
                context.UserAccounts.Add(testUser);
            });

            UserAccount user = _context.UserAccounts.Find(testUser.ID);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.FollowUser(user, "NonExistingUsername")
            );
        }

        [Fact]
        public async Task FollowUser_Throws_IfAlreadyFollowing()
        {
            IEnumerable<UserAccount> testUsers = GenerateUserAccounts(2);
            _context.Setup(context =>
            {
                context.UserAccounts.AddRange(testUsers);

                context.FollowerTable.Add(new FollowRecord
                {
                    Follower = testUsers.First(),
                    Followee = testUsers.Last()
                });
            });

            UserAccount user1 = _context.UserAccounts.Find(testUsers.First().ID);
            UserAccount user2 = _context.UserAccounts.Find(testUsers.Last().ID);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.FollowUser(user1, user2.Username)
            );
        }
        #endregion

        #region UnfollowUser
        [Fact]
        public async Task UnfollowUser_RemovesFollowRecordFromDatabase()
        {
            IEnumerable<UserAccount> testUsers = GenerateUserAccounts(2);
            _context.Setup(context =>
            {
                context.UserAccounts.AddRange(testUsers);

                context.FollowerTable.Add(new FollowRecord
                {
                    Follower = testUsers.First(),
                    Followee = testUsers.Last()
                });
            });

            UserAccount user1 = _context.UserAccounts.Find(testUsers.First().ID);
            UserAccount user2 = _context.UserAccounts.Find(testUsers.Last().ID);

            await _target.UnfollowUser(user1, user2.Username);

            Assert.Empty(user2.Followers);
            Assert.Empty(_context.FollowerTable.ToList());
        }

        [Fact]
        public async Task UnfollowUser_Throws_IfRequestToUnfollowSelf()
        {
            UserAccount testUser = GenerateUserAccounts(1).First();
            _context.Setup(context =>
            {
                context.UserAccounts.Add(testUser);
            });

            UserAccount user1 = _context.UserAccounts.Find(testUser.ID);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.UnfollowUser(user1, user1.Username)
            );
        }

        [Fact]
        public async Task UnfollowUser_Throws_IfUserDoesNotExist()
        {
            UserAccount testUser = GenerateUserAccounts(1).First();
            _context.Setup(context =>
            {
                context.UserAccounts.Add(testUser);
            });

            UserAccount user1 = _context.UserAccounts.Find(testUser.ID);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.UnfollowUser(user1, "NonExistingUsername")
            );
        }

        [Fact]
        public async Task UnfollowUser_Throws_IfNotFollowing()
        {
            IEnumerable<UserAccount> testUsers = GenerateUserAccounts(2);
            _context.Setup(context =>
            {
                context.UserAccounts.AddRange(testUsers);
            });

            UserAccount user1 = _context.UserAccounts.Find(testUsers.First().ID);
            UserAccount user2 = _context.UserAccounts.Find(testUsers.Last().ID);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.UnfollowUser(user1, user2.Username)
            );
        }
        #endregion

        #region GetUserByUsername
        [Fact]
        public async Task GetUserByUsername_RetreivesUserFromDatabase()
        {
            UserAccount testUser = GenerateUserAccounts(1).First();
            _context.Setup(context =>
            {
                context.UserAccounts.Add(testUser);
            });

            UserAccount user = await _target.GetUserByUsername(testUser.Username);

            Assert.Equal(testUser.Username, user.Username);
            Assert.Equal(testUser.ID, user.ID);
        }

        [Fact]
        public async Task GetUserByUsername_Throws_IfUserDoesNotExist()
        {
            UserAccount testAccount = GenerateUserAccounts(1).First();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.GetUserByUsername(testAccount.Username)
            );
        }
        #endregion

        #region GetFollowInfo
        [Fact]
        public async Task GetFollowInfo_Returns_CorrectInformation()
        {
            IEnumerable<UserAccount> testUsers = GenerateUserAccounts(2);
            _context.Setup(context =>
            {
                context.UserAccounts.AddRange(testUsers);

                context.FollowerTable.Add(new FollowRecord
                {
                    Follower = testUsers.First(),
                    Followee = testUsers.Last()
                });
            });

            UserAccount user1 = _context.UserAccounts.Find(testUsers.First().ID);
            UserAccount user2 = _context.UserAccounts.Find(testUsers.Last().ID);

            Assert.Equal((followingCount: 1, followerCount: 0), await _target.GetFollowInfo(user1.Username));
            Assert.Equal((followingCount: 0, followerCount: 1), await _target.GetFollowInfo(user2.Username));
        }
        #endregion

        #region UserIsFollowing 
        [Fact]
        public async Task UserIsFollowing_Returns_TrueIfFollowing()
        {
            IEnumerable<UserAccount> testUsers = GenerateUserAccounts(2);
            _context.Setup(context =>
            {
                context.UserAccounts.AddRange(testUsers);

                context.FollowerTable.Add(new FollowRecord
                {
                    Followee = testUsers.Last(),
                    Follower = testUsers.First()
                });
            });

            UserAccount user1 = _context.UserAccounts.Find(testUsers.First().ID);
            UserAccount user2 = _context.UserAccounts.Find(testUsers.Last().ID);

            Assert.True(await _target.UserIsFollowing(user1, user2.Username));
        }

        [Fact]
        public async Task UserIsFollowing_Returns_FalseIfNotFollowing()
        {
            IEnumerable<UserAccount> testUsers = GenerateUserAccounts(2);
            _context.Setup(context =>
            {
                context.UserAccounts.AddRange(testUsers);
            });

            UserAccount user1 = _context.UserAccounts.Find(testUsers.First().ID);
            UserAccount user2 = _context.UserAccounts.Find(testUsers.Last().ID);

            Assert.False(await _target.UserIsFollowing(user1, user2.Username));
        }
        #endregion

        public void Dispose() => _context.Dispose();
    }
}
