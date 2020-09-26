using Bulliten.API.Models;
using Bulliten.API.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Bulliten.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly BullitenDBContext _context;

        public PostController(ILogger<PostController> logger, BullitenDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("feed/public")]
        public async Task<IActionResult> GetPublicFeed([FromQuery] string username)
        {
            UserAccount user = await _context.UserAccounts.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return BadRequest(new JsonError("User does not exist"));

            IEnumerable<Post> userPosts = await QueryPosts(q =>
                q.Where(p => p.Author.ID == user.ID));

            IEnumerable<Post> reposted = await QueryPosts(q =>
                q.Where(p => p.RepostedBy.Any(ur => ur.UserId == user.ID)));

            IEnumerable<Post> posts = userPosts
                .Concat(reposted)
                .NoDuplicates()
                .OrderByDescending(p => p.CreationDate)
                .ToList();

            posts
                .AsParallel()
                .ForAll(p => p.PopulateStatuses(GetAccountFromContext()));

            return Ok(new { posts });
        }

        [HttpGet("feed/personal")]
        public async Task<IActionResult> GetPersonalFeed()
        {
            UserAccount user = GetAccountFromContext();

            IEnumerable<Post> userPosts = await QueryPosts(q =>
                q.Where(p => p.Author.ID == user.ID));

            IEnumerable<int> userFollowingIds = await _context.FollowerTable
                .AsNoTracking()
                .Where(fr => fr.FollowerId == user.ID)
                .Select(fr => fr.FolloweeId)
                .ToListAsync();

            IEnumerable<Post> followedUsersPosts = await QueryPosts(q =>
                q.Where(p => userFollowingIds.Contains(p.Author.ID))
            );

            IEnumerable<Post> reposted = await QueryPosts(q =>
                q.Where(p => p.RepostedBy
                                .Any(ur => ur.UserId == user.ID)
                )
            );

            IEnumerable<Post> orderedPosts = userPosts
                .Concat(followedUsersPosts)
                .Concat(reposted)
                .NoDuplicates()
                .OrderByDescending(p => p.CreationDate);

            orderedPosts
                .AsParallel()
                .ForAll(p => p.PopulateStatuses(user));

            return Ok(new { posts = orderedPosts });
        }

        [HttpPost("like")]
        public async Task<IActionResult> LikePost(int postId)
        {
            return await ActOnPost(
                postId,
                new List<string> { "LikedPosts" },
                (post, user) =>
                {
                    if (user.LikedPosts.Any(ul => ul.PostId == post.ID))
                        return BadRequest(new JsonError("Cannot like a post you already liked"));

                    user.LikedPosts.Add(new UserLike { Post = post, User = user });
                    post.Likes++;

                    return Ok();
                }
            );
        }

        [HttpPost("unlike")]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            return await ActOnPost(
                postId,
                new List<string> { "LikedPosts" },
                (post, user) =>
                {
                    UserLike userLikeToRemove = user.LikedPosts.SingleOrDefault(ul => ul.PostId == post.ID);
                    bool likeWasRemoved = user.LikedPosts.Remove(userLikeToRemove);

                    if (likeWasRemoved)
                        post.Likes--;

                    return Ok();
                }
            );
        }

        [HttpPost("repost")]
        public async Task<IActionResult> RePost(int postId)
        {
            return await ActOnPost(
                postId,
                new List<string> { "RePosts" },
                (post, user) =>
                {
                    if (user.RePosts.Any(ur => ur.PostId == post.ID))
                        return BadRequest(new JsonError("Cannot repost a post you already reposted"));

                    user.RePosts.Add(new UserRepost { Post = post, User = user });
                    post.RePosts++;

                    return Ok();
                }
            );
        }

        [HttpPost("unrepost")]
        public async Task<IActionResult> UnRePost(int postId)
        {
            return await ActOnPost(
                postId,
                new List<string> { "RePosts" },
                (post, user) =>
                {
                    UserRepost userRepostToRemove = user.RePosts.SingleOrDefault(ur => ur.PostId == post.ID);
                    bool repostWasRemoved = user.RePosts.Remove(userRepostToRemove);

                    if (repostWasRemoved)
                        post.RePosts--;

                    return Ok();
                }
            );
        }

        [HttpPost("create")]
        public async Task CreatePost([FromForm] Post formPost)
        {
            UserAccount dbUser = await _context.UserAccounts.SingleAsync(u => u.ID == GetAccountFromContext().ID);

            dbUser.Posts.Add(formPost);

            await _context.SaveChangesAsync();

            Ok();
        }

        private UserAccount GetAccountFromContext() =>
            (UserAccount)HttpContext.Items[JwtMiddleware.CONTEXT_USER];

        /// <summary>
        /// Applies the passed in function delegate parameter to a specific post
        /// </summary>
        /// <param name="postId">Id of the post to apply the function delegate to</param>
        /// <param name="includeProps">Dependent properties to load from the database for the <see cref="UserAccount"/> object in the function delegate</param>
        /// <param name="funcDelegate">The method applied to the post parameter</param>
        /// <returns></returns>
        private async Task<IActionResult> ActOnPost(
            int postId,
            IEnumerable<string> includeProps,
            Func<Post, UserAccount, IActionResult> funcDelegate)
        {
            Post post = await _context.Posts.SingleOrDefaultAsync(p => p.ID == postId);

            if (post == null) return BadRequest(new JsonError("Post with provided ID does not exist"));

            UserAccount user = await _context.UserAccounts.SingleOrDefaultAsync(u => u.ID == GetAccountFromContext().ID);

            foreach (string propName in includeProps)
                await _context.Entry(user).Collection(propName).LoadAsync();

            IActionResult result = funcDelegate(post, user);

            await _context.SaveChangesAsync();

            return result;
        }

        /// <summary>
        /// Adds the passed in filters to a post query 
        /// </summary>
        /// <param name="filters">The filters to be applied to the query</param>
        /// <returns></returns>
        private async Task<IEnumerable<Post>> QueryPosts(Func<IQueryable<Post>, IQueryable<Post>> filters)
        {
            IQueryable<Post> query = _context.Posts
                .AsNoTracking()
                .Include(p => p.Author)
                .Include(p => p.LikedBy)
                .Include(p => p.RepostedBy)
                .AsQueryable();

            IQueryable<Post> newQuery = filters(query);

            return await newQuery.ToListAsync();
        }
    }
}
