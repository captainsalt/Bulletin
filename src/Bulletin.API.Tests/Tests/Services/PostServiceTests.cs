﻿using Bulletin.API.Models;
using Bulletin.API.Services;
using Bulletin.API.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Bulletin.API.Tests.Helpers.TestHelpers;

namespace Bulletin.API.Tests.Services
{
    public class PostServiceTests : IDisposable
    {
        private readonly BulletinDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PostService _target;

        public PostServiceTests()
        {
            var globalMocks = new GlobalMocks();
            _context = new ConnectionFactory().CreateContextForSQLite();

            _httpContextAccessor = globalMocks.HttpContextAccessor;

            _target = new PostService(_context, _httpContextAccessor);
        }

        public void Dispose() => _context.Dispose();

        #region GetPublicFeed
        [Fact]
        public async Task GetPublicFeed_Returns_RepostedPosts()
        {
            _context.AddUsers(2)
                .Setup(context =>
                {
                    UserAccount user1 = _context.GetUserById(1);
                    UserAccount user2 = _context.GetUserById(2);

                    Post user2Post = GenerateRandomPosts(1).First();

                    user2.Posts.Add(user2Post);

                    context.UserReposts.Add(new UserRepost
                    {
                        Post = user2Post,
                        User = user1
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
            _context.AddUsers(1)
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

        #region GetPersonalFeed
        [Fact]
        public async Task GetPersonalFeed_Returns_OwnPosts()
        {
            _context.AddUsers(1)
                .Setup(context =>
                {
                    UserAccount user = _context.GetUserById(1);
                    Post user1Post = GenerateRandomPosts(1).First();

                    user.Posts.Add(user1Post);
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            IEnumerable<Post> feed = await _target.GetPersonalFeed();

            Assert.Single(feed);
        }

        [Fact]
        public async Task GetPersonalFeed_Returns_FolloweePosts()
        {
            _context.AddUsers(2)
                .Setup(context =>
                {
                    UserAccount user1 = _context.GetUserById(1);
                    UserAccount user2 = _context.GetUserById(2);
                    Post user2Post = GenerateRandomPosts(1).First();

                    user2.Posts.Add(user2Post);

                    context.FollowerTable.Add(new FollowRecord
                    {
                        Follower = user1,
                        Followee = user2
                    });
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            IEnumerable<Post> feed = await _target.GetPersonalFeed();

            Assert.Single(feed);
        }

        [Fact]
        public async Task GetPersonalFeed_Returns_RepostedPosts()
        {
            _context.AddUsers(2)
                .Setup(context =>
                {
                    UserAccount user1 = _context.GetUserById(1);
                    UserAccount user2 = _context.GetUserById(2);
                    Post user2Post = GenerateRandomPosts(1).First();

                    user2.Posts.Add(user2Post);

                    context.UserReposts.Add(new UserRepost
                    {
                        User = user1,
                        Post = user2Post
                    });
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            IEnumerable<Post> feed = await _target.GetPersonalFeed();

            Assert.Single(feed);
        }

        [Fact]
        public async Task GetPersonalFeed_Returns_RepostStatus()
        {
            _context.AddUsers(2)
                .Setup(context =>
                {
                    UserAccount user = _context.GetUserById(1);
                    Post userPost = GenerateRandomPosts(1).First();

                    user.Posts.Add(userPost);

                    context.UserReposts.Add(new UserRepost
                    {
                        User = user,
                        Post = userPost
                    });
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            IEnumerable<Post> feed = await _target.GetPersonalFeed();
            bool repostStatus = feed.Single().RePostStatus;

            Assert.True(repostStatus);
        }

        [Fact]
        public async Task GetPersonalFeed_Returns_LikeStatus()
        {
            _context.AddUsers(2)
                .Setup(context =>
                {
                    UserAccount user = _context.GetUserById(1);
                    Post userPost = GenerateRandomPosts(1).First();

                    user.Posts.Add(userPost);

                    context.UserLike.Add(new UserLike
                    {
                        User = user,
                        Post = userPost
                    });
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            IEnumerable<Post> feed = await _target.GetPersonalFeed();
            bool likeStatus = feed.Single().LikeStatus;

            Assert.True(likeStatus);
        }
        #endregion

        #region LikePost
        [Fact]
        public async Task LikePost_Adds_LikeToDatabase()
        {
            _context.AddUsers(1)
                .Setup(context =>
                {
                    UserAccount user = _context.GetUserById(1);
                    Post userPost = GenerateRandomPosts(1).First();

                    user.Posts.Add(userPost);
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            await _target.LikePost(1);

            var likedPosts = _context.UserLike.ToList();

            Assert.Single(likedPosts);
        }

        [Fact]
        public async Task LikePost_Throws_IfPostAlreadyLiked()
        {
            _context.AddUsers(1)
                .Setup(context =>
                {
                    UserAccount user = _context.GetUserById(1);
                    Post post = GenerateRandomPosts(1).First();

                    user.Posts.Add(post);

                    context.UserLike.Add(new UserLike
                    {
                        User = user,
                        Post = post
                    });
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.LikePost(1)
            );
        }

        [Fact]
        public async Task LikePost_Adds_OneToLikeCount()
        {
            _context.AddUsers(1)
                .Setup(context =>
                {
                    UserAccount user = _context.GetUserById(1);
                    Post userPost = GenerateRandomPosts(1).First();

                    user.Posts.Add(userPost);
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            await _target.LikePost(1);

            Post likedPost = _context.UserLike.Single().Post;

            Assert.Equal(1, likedPost.Likes);
        }
        #endregion

        #region RemoveLike
        [Fact]
        public async Task RemoveLike_Removes_LikeFromDatabase()
        {
            _context.AddUsers(1)
                .Setup(context =>
                {
                    UserAccount user = _context.GetUserById(1);
                    Post userPost = GenerateRandomPosts(1).First();

                    user.Posts.Add(userPost);

                    context.UserLike.Add(new UserLike
                    {
                        Post = userPost,
                        User = user
                    });
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            await _target.RemoveLike(1);

            var likedPosts = _context.UserLike.ToList();

            Assert.Empty(likedPosts);
        }

        [Fact]
        public async Task RemoveLike_Throws_IfPostIsNotLiked()
        {
            _context.AddUsers(1)
                .Setup(context =>
                {
                    UserAccount user = _context.GetUserById(1);
                    Post userPost = GenerateRandomPosts(1).First();

                    user.Posts.Add(userPost);
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.RemoveLike(1)
            );
        }

        [Theory]
        [InlineData(0, 1)]
        public async Task RemoveLike_Removes_OneFromLikeCount(int expected, int initial)
        {
            _context.AddUsers(1)
                .Setup(context =>
                {
                    UserAccount user = _context.GetUserById(1);
                    Post userPost = GenerateRandomPosts(1).First();

                    userPost.Likes = initial;

                    user.Posts.Add(userPost);

                    context.UserLike.Add(new UserLike
                    {
                        User = user,
                        Post = userPost
                    });
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            await _target.RemoveLike(1);

            Post unlikedPost = _context.Posts.First();

            Assert.Equal(expected, unlikedPost.Likes);
        }
        #endregion

        #region RePost
        [Fact]
        public async Task RePost_Adds_PostToDatabase()
        {
            _context.AddUsers(1)
                .Setup(context =>
                {
                    UserAccount user = _context.GetUserById(1);
                    Post userPost = GenerateRandomPosts(1).First();

                    user.Posts.Add(userPost);
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            await _target.RePost(1);

            var reposts = _context.UserReposts.ToList();

            Assert.Single(reposts);
        }

        [Fact]
        public async Task RePost_Throws_IfPostIsAlreadyRePosted()
        {
            _context.AddUsers(1)
                .Setup(context =>
                {
                    UserAccount user = _context.GetUserById(1);
                    Post userPost = GenerateRandomPosts(1).First();

                    user.Posts.Add(userPost);

                    context.UserReposts.Add(new UserRepost
                    {
                        User = user,
                        Post = userPost
                    });
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.RePost(1)
            );
        }
        #endregion

        #region RemoveRepost
        [Fact]
        public async Task RemoveRePost_Removes_RePostFromDatabase()
        {
            _context.AddUsers(1)
                .Setup(context =>
                {
                    UserAccount user = _context.GetUserById(1);
                    Post userPost = GenerateRandomPosts(1).First();

                    user.Posts.Add(userPost);

                    context.UserReposts.Add(new UserRepost
                    {
                        User = user,
                        Post = userPost
                    });
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            await _target.RemoveRePost(1);

            var reposts = _context.UserReposts.ToList();

            Assert.Empty(reposts);
        }

        [Fact]
        public async Task RemoveRePost_Throws_IfPostIsNotRePosted()
        {
            _context.AddUsers(1)
                .Setup(context =>
                {
                    UserAccount user = _context.GetUserById(1);
                    Post userPost = GenerateRandomPosts(1).First();

                    user.Posts.Add(userPost);
                });

            UserAccount contextUser = _context.GetUserById(1);

            _httpContextAccessor.SetContextUser(contextUser);

            var reposts = _context.UserReposts.ToList();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _target.RemoveRePost(1)
            );
        }
        #endregion

        #region CreatePost 
        [Fact]
        public async Task CreatePost_Adds_PostToDatabase()
        {
            _context.AddUsers(1);

            UserAccount contextUser = _context.GetUserById(1);
            Post post = GenerateRandomPosts(1).First();

            _httpContextAccessor.SetContextUser(contextUser);

            await _target.CreatePost(post);

            var posts = _context.Posts.ToList();

            Assert.Single(posts);
        }
        #endregion
    }
}