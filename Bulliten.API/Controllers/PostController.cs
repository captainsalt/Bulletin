﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bulliten.API.Models;
using Bulliten.API.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Bulliten.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly BullitenDBContext _context;

        public PostController(BullitenDBContext context)
        {
            _context = context;
        }

        // GET: api/<PostController>
        [HttpGet]
        public IActionResult GetPosts()
        {
            var ctxUser = GetAccountFromContext();
            var posts = _context.Posts.Where(p => p.Author.ID == ctxUser.ID);

            return Ok(new { posts });
        }

        // GET api/<PostController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PostController>
        [HttpPost("create")]
        public async Task CreatePost([FromForm] Post formPost)
        {
            var ctxUser = GetAccountFromContext();
            var dbUser = await _context.UserAccounts.SingleAsync(u => u.ID == ctxUser.ID);

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

        private UserAccount GetAccountFromContext() =>
            (UserAccount)HttpContext.Items[JwtMiddleware.CONTEXT_USER];

    }
}
