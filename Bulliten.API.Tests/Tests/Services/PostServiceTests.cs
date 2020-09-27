using Bulliten.API.Middleware;
using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using Bulliten.API.Services;
using Bulliten.API.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Bulliten.API.Tests.Helpers.TestHelpers;

namespace Bulliten.API.Tests.Services
{
    public class PostServiceTests
    {
        private BullitenDBContext _context;
        private IHttpContextAccessor _httpContextAccessor;
        private PostService _target;

        public PostServiceTests()
        {
            var globalMocks = new GlobalMocks();
            _context = new ConnectionFactory().CreateContextForSQLite();

            _httpContextAccessor = globalMocks.HttpContextAccessor;

            _target = new PostService(_context, _httpContextAccessor);
        }

        #region GetPublicFeed
        [Fact]
        public async Task GetPublicFeed_Returns_Reposted()
        {
            _context.AddRandomUsers(2)
                .Setup(context =>
                {
                    UserAccount user1 = _context.GetUserById(1);
                    UserAccount user2 = _context.GetUserById(2);

                    Post user2Post = GenerateRandomPosts(1).First();

                    user2.Posts.Add(user2Post);

                    user1.RePosts.Add(new UserRepost
                    {
                        Post = user2Post
                    });
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            IEnumerable<Post> feed = await _target.GetPublicFeed(contextUser.Username);

            Assert.Single(feed);
        }

        [Fact]
        public async Task GetPublicFeed_Returns_OwnPosts()
        {
            _context.AddRandomUsers(1)
                .Setup(context =>
                {
                    UserAccount user = _context.GetUserById(1);

                    Post userPost = GenerateRandomPosts(1).First();

                    user.Posts.Add(userPost);
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            IEnumerable<Post> feed = await _target.GetPublicFeed(contextUser.Username);

            Assert.Single(feed);
        }
        #endregion
    }
}