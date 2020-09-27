using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using Bulliten.API.Services;
using Bulliten.API.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Bulliten.API.Tests.Helpers.TestHelpers;

namespace Bulliten.API.Tests.Services
{
    public class UserServiceTests : IDisposable
    {
        private readonly UserAccountService _target;
        private readonly BullitenDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserServiceTests()
        {
            var globalMocks = new GlobalMocks();
            _context = new ConnectionFactory().CreateContextForSQLite();

            _httpContextAccessor = globalMocks.HttpContextAccessor;

            _target = new UserAccountService(
                _context,
                globalMocks.AuthenticationService,
                _httpContextAccessor
            );
        }

        #region CreateAccount
        [Fact]
        public async Task CreateAccount_Adds_UserToDababase()
        {
            UserAccount testAccount = GenerateUserAccounts(1).First();
            await _target.CreateAccount(testAccount);

            Assert.NotNull(_context.UserAccounts.ToList());
        }

        [Fact]
        public async Task CreateAccount_Returns_Credentials()
        {
            UserAccount testAccount = GenerateUserAccounts(1).First();
            AuthenticationResponse response = await _target.CreateAccount(testAccount);

            Assert.Equal(testAccount.Username, response.User.Username);
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
        public async Task Login_Returns_Credentials()
        {
            UserAccount testAccount = GenerateUserAccounts(1).First();
            _context.Setup(context =>
            {
                context.UserAccounts.Add(testAccount);
            });

            AuthenticationResponse response = await _target.Login(testAccount);

            Assert.NotNull(response.User);
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

        [Fact]
        public async Task Login_Throws_IfUserDoesNotExist()
        {
            UserAccount testAccount = GenerateUserAccounts(1).First();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.Login(testAccount)
            );
        }
        #endregion

        #region FollowUser
        [Fact]
        public async Task FollowUser_Adds_FollowRecordToDatabase()
        {
            IEnumerable<UserAccount> users = GenerateUserAccounts(2);
            _context.Setup(context =>
            {
                context.UserAccounts.AddRange(users);
            });

            UserAccount user1 = _context.UserAccounts.Find(users.First().ID);
            UserAccount user2 = _context.UserAccounts.Find(users.Last().ID);

            _httpContextAccessor.SetContextUser(user1);

            await _target.FollowUser(user2.Username);

            FollowRecord followRecord = _context.FollowerTable.FirstOrDefault();

            Assert.NotNull(followRecord);
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

            _httpContextAccessor.SetContextUser(user);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _target.FollowUser(user.Username)
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

            _httpContextAccessor.SetContextUser(user);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.FollowUser("NonExistingUsername")
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

            _httpContextAccessor.SetContextUser(user1);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.FollowUser(user2.Username)
            );
        }
        #endregion

        #region UnfollowUser
        [Fact]
        public async Task UnfollowUser_Removes_FollowRecordFromDatabase()
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

            _httpContextAccessor.SetContextUser(user1);

            await _target.UnfollowUser(user2.Username);

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

            _httpContextAccessor.SetContextUser(user1);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.UnfollowUser(user1.Username)
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

            _httpContextAccessor.SetContextUser(user1);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.UnfollowUser("NonExistingUsername")
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

            _httpContextAccessor.SetContextUser(user1);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.UnfollowUser(user2.Username)
            );
        }
        #endregion

        #region GetUserByUsername
        [Fact]
        public async Task GetUserByUsername_Returns_User()
        {
            UserAccount testUser = GenerateUserAccounts(1).First();
            _context.Setup(context =>
            {
                context.UserAccounts.Add(testUser);
            });

            UserAccount user = await _target.GetUserByUsername(testUser.Username);

            Assert.Equal(testUser.Username, user.Username);
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

            _httpContextAccessor.SetContextUser(user1);

            Assert.True(await _target.UserIsFollowing(user2.Username));
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

            Assert.False(await _target.UserIsFollowing(user2.Username));
        }
        #endregion

        public void Dispose() => _context.Dispose();
    }
}
