using Bulliten.API.Middleware;
using Bulliten.API.Models;
using Bulliten.API.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
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
            UserAccount user = await _context.UserAccounts.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                throw new ArgumentException("User does not exist");

            IEnumerable<Post> userPosts = user.Posts;

            List<Post> reposted = await _context.UserReposts
                .AsNoTracking()
                .Where(ur => ur.UserId == user.ID)
                .Select(ur => ur.Post)
                .ToListAsync();

            IEnumerable<Post> posts = userPosts
                .Concat(reposted)
                .NoDuplicates()
                .OrderByDescending(p => p.CreationDate)
                .ToList();

            return posts;
        }

        public async Task<IEnumerable<Post>> GetPersonalFeed()
        {
            UserAccount user = GetContextUser();

            IEnumerable<Post> userPosts = user.Posts;

            List<Post> followeePosts = await (
                from fr in _context.FollowerTable.AsNoTracking()
                where fr.FollowerId == user.ID
                join p in _context.Posts
                on fr.FolloweeId equals p.AuthorId
                select p
            ).ToListAsync();

            List<Post> reposted = await _context.UserReposts
                .AsNoTracking()
                .Where(ur => ur.UserId == user.ID)
                .Select(ur => ur.Post)
                .ToListAsync();

            IEnumerable<Post> orderedPosts = userPosts
                .Concat(followeePosts)
                .Concat(reposted)
                .NoDuplicates()
                .OrderByDescending(p => p.CreationDate);

            return orderedPosts;
        }

        public async Task LikePost(int postId)
        {
            await ActOnPost(
                postId,
                new List<string> { "LikedPosts" },
                (post, user) =>
                {
                    if (user.LikedPosts.Any(ul => ul.PostId == post.ID))
                        throw new ArgumentException("Cannot like a post you already liked");

                    user.LikedPosts.Add(new UserLike { Post = post, User = user });
                    post.Likes++;
                }
            );
        }

        public async Task RemoveLike(int postId)
        {
            await ActOnPost(
                postId,
                new List<string> { "LikedPosts" },
                (post, user) =>
                {
                    UserLike userLikeToRemove = user.LikedPosts.SingleOrDefault(ul => ul.PostId == post.ID);
                    bool likeWasRemoved = user.LikedPosts.Remove(userLikeToRemove);


                    if (likeWasRemoved)
                        post.Likes--;
                    else
                        throw new ArgumentException("Cannot unlike a post you did not like");
                }
            );
        }

        public async Task RePost(int postId)
        {
            await ActOnPost(
                postId,
                new List<string> { "RePosts" },
                (post, user) =>
                {
                    if (user.RePosts.Any(ur => ur.PostId == post.ID))
                        throw new ArgumentException("Cannot repost a post you already reposted");

                    user.RePosts.Add(new UserRepost { Post = post, User = user });
                    post.RePosts++;
                }
            );
        }

        public async Task RemoveRePost(int postId)
        {
            await ActOnPost(
                postId,
                new List<string> { "RePosts" },
                (post, user) =>
                {
                    UserRepost userRepostToRemove = user.RePosts.SingleOrDefault(ur => ur.PostId == post.ID);
                    bool repostWasRemoved = user.RePosts.Remove(userRepostToRemove);

                    if (repostWasRemoved)
                        post.RePosts--;
                    else
                        throw new ArgumentException("Cannot unrepost a post you did not repost");
                }
            );
        }

        public async Task CreatePost(Post formPost)
        {
            UserAccount dbUser = _context.UserAccounts.Find(GetContextUser().ID);

            dbUser.Posts.Add(formPost);

            await _context.SaveChangesAsync();
        }

        private UserAccount GetContextUser() =>
            (UserAccount)_httpContextAccessor.HttpContext.Items[JwtMiddleware.CONTEXT_USER];

        /// <summary>
        /// Applies the passed in function delegate parameter to a specific post
        /// </summary>
        /// <param name="postId">Id of the post to apply the function delegate to</param>
        /// <param name="includeProps">Dependent properties to load from the database for the <see cref="UserAccount"/> object in the function delegate</param>
        /// <param name="action">The method applied to the post parameter</param>
        /// <returns></returns>
        private async Task ActOnPost(
            int postId,
            IEnumerable<string> includeProps,
            Action<Post, UserAccount> action)
        {
            Post post = await _context.Posts.SingleOrDefaultAsync(p => p.ID == postId);

            if (post == null)
                throw new ArgumentException("Post with provided ID does not exist");

            UserAccount user = await _context
                .UserAccounts
                .SingleOrDefaultAsync(u => u.ID == GetContextUser().ID);

            foreach (string propName in includeProps)
                await _context.Entry(user).Collection(propName).LoadAsync();

            action(post, user);

            await _context.SaveChangesAsync();
        }
    }
}
