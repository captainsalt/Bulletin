using Bulliten.API.Middleware;
using Bulliten.API.Models;
using Bulliten.API.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Bulliten.API.Services
{
    public class PostService : IPostService
    {
        private readonly BullitenDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostService(BullitenDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Post>> GetPublicFeed(string username)
        {
            UserAccount contextUser = await _context.UserAccounts.SingleOrDefaultAsync(u => u.Username == username);

            if (contextUser == null)
                throw new ArgumentException("User does not exist");

            await _context.Entry(contextUser)
                .Collection(u => u.Posts)
                .LoadAsync();

            IEnumerable<Post> userPosts = contextUser.Posts.ToList();

            List<Post> reposted = await _context.UserReposts
                .AsNoTracking()
                .Where(ur => ur.UserId == contextUser.ID)
                .Select(ur => ur.Post)
                .ToListAsync();

            IEnumerable<Post> posts = userPosts
                .Concat(reposted)
                .NoDuplicates()
                .OrderByDescending(p => p.CreationDate);

            PopulatePostStatuses(posts);

            return posts;
        }

        public async Task<IEnumerable<Post>> GetPersonalFeed()
        {
            UserAccount contextUser = GetContextUser();

            await _context.Entry(contextUser)
                .Collection(u => u.Posts)
                .LoadAsync();

            var userPosts = contextUser.Posts.ToList();

            List<Post> followeePosts = await
                (from fr in _context.FollowerTable.AsNoTracking()
                 where fr.FollowerId == contextUser.ID
                 join p in _context.Posts.AsNoTracking()
                 on fr.FolloweeId equals p.AuthorId
                 select p)
                .Include(p => p.Author)
                .ToListAsync();

            List<Post> reposted = await
                (from ur in _context.UserReposts.AsNoTracking()
                 where ur.UserId == contextUser.ID
                 join p in _context.Posts.AsNoTracking()
                 on ur.PostId equals p.ID
                 select p)
                .Include(p => p.Author)
                .ToListAsync();

            IEnumerable<Post> orderedPosts = userPosts
                .Concat(followeePosts)
                .Concat(reposted)
                .NoDuplicates()
                .OrderByDescending(p => p.CreationDate);

            PopulatePostStatuses(orderedPosts);

            return orderedPosts;
        }

        public async Task LikePost(int postId)
        {
            await ActOnPost(
                postId,
                async (post, user) =>
                {
                    await _context.Entry(user)
                        .Collection(u => u.LikedPosts)
                        .LoadAsync();

                    if (user.LikedPosts.Any(ul => ul.PostId == post.ID))
                        throw new ArgumentException("Cannot like a post you already liked");

                    user.LikedPosts.Add(new UserLike { Post = post, User = user });

                    int likeCount = await GetPostLikeCount(postId);
                    post.Likes = likeCount + 1;
                }
            );
        }

        public async Task RemoveLike(int postId)
        {
            await ActOnPost(
                postId,
                async (post, user) =>
                {
                    await _context.Entry(user)
                        .Collection(u => u.LikedPosts)
                        .LoadAsync();

                    UserLike userLikeToRemove = await _context.UserLike
                        .SingleOrDefaultAsync(ul => ul.PostId == postId && ul.UserId == user.ID);

                    bool likeWasRemoved = user.LikedPosts.Remove(userLikeToRemove);

                    if (likeWasRemoved)
                    {
                        int likeCount = await GetPostLikeCount(post.ID);
                        post.Likes = likeCount - 1;
                    }
                    else
                    {
                        throw new ArgumentException("Cannot unlike a post you did not like");
                    }
                }
            );
        }

        public async Task RePost(int postId)
        {
            await ActOnPost(
                postId,
                async (post, user) =>
                {
                    await _context.Entry(user)
                        .Collection(u => u.RePosts)
                        .LoadAsync();

                    if (user.RePosts.Any(ur => ur.PostId == post.ID))
                        throw new ArgumentException("Cannot repost a post you already reposted");

                    user.RePosts.Add(new UserRepost { Post = post, User = user });

                    int repostCount = await GetPostRePostCount(postId);
                    post.RePosts = repostCount + 1;
                }
            );
        }

        public async Task RemoveRePost(int postId)
        {
            await ActOnPost(
                postId,
                async (post, user) =>
                {
                    await _context.Entry(user)
                        .Collection(u => u.RePosts)
                        .LoadAsync();

                    UserRepost userRepostToRemove = await _context.UserReposts
                        .SingleOrDefaultAsync(ur => ur.PostId == postId && ur.UserId == user.ID);

                    bool repostWasRemoved = user.RePosts.Remove(userRepostToRemove);

                    if (repostWasRemoved)
                    {
                        int repostCount = await GetPostRePostCount(postId);
                        post.RePosts = repostCount - 1;
                    }
                    else
                    {
                        throw new ArgumentException("Cannot unrepost a post you did not repost");
                    }
                }
            );
        }

        public async Task CreatePost(Post formPost)
        {
            UserAccount dbUser = await _context.UserAccounts.FindAsync(GetContextUser().ID);

            dbUser.Posts.Add(formPost);

            await _context.SaveChangesAsync();
        }

        private void PopulatePostStatuses(IEnumerable<Post> posts)
        {
            IEnumerable<Post> repostedByContextUser =
                (from p in posts
                 join ur in _context.UserReposts
                 on p.ID equals ur.PostId
                 select p)
                 .ToList();

            IEnumerable<Post> likedByContextUser =
                (from p in posts
                 join ul in _context.UserLike
                 on p.ID equals ul.PostId
                 select p)
                 .ToList();

            posts.AsParallel().ForAll(post =>
            {
                if (repostedByContextUser.Contains(post))
                    post.RePostStatus = true;

                if (likedByContextUser.Contains(post))
                    post.LikeStatus = true;
            });
        }

        private UserAccount GetContextUser() =>
            (UserAccount)_httpContextAccessor.HttpContext.Items[JwtMiddleware.CONTEXT_USER];

        /// <summary>
        /// Applies the passed in function delegate parameter to a specific post
        /// </summary>
        /// <param name="postId">Id of the post to apply the function delegate to</param>
        /// <param name="action">The method applied to the post parameter</param>
        /// <returns></returns>
        private async Task ActOnPost(
            int postId,
            Func<Post, UserAccount, Task> action)
        {
            Post post = await _context.Posts.FindAsync(postId);

            if (post == null)
                throw new ArgumentException("Post with provided ID does not exist");

            UserAccount user = await _context
                .UserAccounts
                .FindAsync(GetContextUser().ID);

            await action(post, user);

            await _context.SaveChangesAsync();
        }

        private async Task<int> GetPostLikeCount(int postId) =>
            await _context.UserLike.Where(ul => ul.PostId == postId).CountAsync();

        private async Task<int> GetPostRePostCount(int postId) =>
            await _context.UserReposts.Where(ur => ur.PostId == postId).CountAsync();
    }
}
