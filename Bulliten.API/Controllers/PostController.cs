using Bulliten.API.Middleware;
using Bulliten.API.Models;
using Bulliten.API.Models.Server;
using Bulliten.API.Services;
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
        private readonly IPostService _postService;

        public PostController(
            ILogger<PostController> logger,
            IPostService postService)
        {
            _logger = logger;
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
                IEnumerable<Post> posts = await _postService.GetPersonalFeed();
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

        [HttpDelete("like/remove")]
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

        [HttpDelete("repost/remove")]
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
        public async Task<IActionResult> CreatePost([FromForm] Post formPost)
        {
            try
            {
                await _postService.CreatePost(formPost);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
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
    }
}
