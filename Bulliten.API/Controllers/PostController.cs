﻿using Bulliten.API.Models;
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
                return BadRequest(new Error("User does not exist"));

            IEnumerable<Post> posts = await _context.Posts.ToListAsync();

            return Ok(new { posts });
        }

        [HttpGet("feed/personal")]
        public async Task<IActionResult> GetPersonalFeed()
        {
            UserAccount user = GetAccountFromContext();

            IEnumerable<int> following = await _context.FollowerTable
                .Where(fr => fr.FollowerId == user.ID)
                .Select(fr => fr.FolloweeId)
                .ToListAsync();

            IEnumerable<Post> posts = await _context.Posts.Include(p => p.Author).ToListAsync();

            IEnumerable<Post> userPosts = posts.Where(p => p.Author.ID == user.ID).ToList();
            IEnumerable<Post> followingPosts = posts.Where(p => following.Contains(p.Author.ID)).ToList();

            IEnumerable<Post> orderedPosts = userPosts
                .Concat(followingPosts)
                .OrderByDescending(p => p.CreationDate);

            return Ok(new { posts = orderedPosts });
        }

        // GET api/<PostController>/5
        [HttpGet("{id}")]
        public string Get(int id) => "value";

        // POST api/<PostController>
        [HttpPost("create")]
        public async Task CreatePost([FromForm] Post formPost)
        {
            UserAccount ctxUser = GetAccountFromContext();
            UserAccount dbUser = await _context.UserAccounts.SingleAsync(u => u.ID == ctxUser.ID);

            formPost.Author = ctxUser;
            formPost.CreationDate = DateTime.Now;

            dbUser.Posts.Add(formPost);
            await _context.SaveChangesAsync();

            Ok();
        }

        // PUT api/<PostController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PostController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private UserAccount GetAccountFromContext() => (UserAccount)HttpContext.Items[JwtMiddleware.CONTEXT_USER];
    }
}
