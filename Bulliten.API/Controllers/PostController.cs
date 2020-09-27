using Bulliten.API.Middleware;
using Bulliten.API.Models;
using Bulliten.API.Models.Server;
using Bulliten.API.Services;
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
        private readonly IPostService _postService;

        public PostController(
            ILogger<PostController> logger,
            BullitenDBContext context,
            IPostService postService)
        {
            _logger = logger;
            _context = context;
            _postService = postService;
        }

        [HttpGet("feed/public")]
        public async Task<IActionResult> GetPublicFeed([FromQuery] string username)
        {
            try
            {
                IEnumerable<Post> posts = await _postService.GetPublicFeed(username);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("feed/personal")]
        public async Task<IActionResult> GetPersonalFeed()
        {
            try
            {
                var posts = await _postService.GetPersonalFeed();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("like")]
        public async Task<IActionResult> LikePost(int postId)
        {
            try
            {
                await _postService.LikePost(postId);
                return Ok();
            }
            catch (Exception ex)   
            {
                return HandleException(ex);
            }
        }

        [HttpPost("unlike")]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            try
            {
                await _postService.RemoveLike(postId);  
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("repost")]
        public async Task<IActionResult> RePost(int postId)
        {
            try
            {
                await _postService.RePost(postId);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("unrepost")]
        public async Task<IActionResult> UnRePost(int postId)
        {
            try
            {
                await _postService.RemoveRePost(postId);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("create")]
        public async Task CreatePost([FromForm] Post formPost)
        {
            UserAccount dbUser = await _context.UserAccounts.SingleAsync(u => u.ID == GetAccountFromContext().ID);

            dbUser.Posts.Add(formPost);

            await _context.SaveChangesAsync();

            Ok();
        }

        private IActionResult HandleException(Exception ex)
        {
            if (ex is ArgumentException argEx)
            {
                return BadRequest(new JsonError(argEx.Message));
            }
            else
            {
                _logger.LogCritical(ex, "Internal server error");
                return Problem("An internal server error has occured");
            }
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
