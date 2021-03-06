﻿using Bulletin.API.Middleware;
using Bulletin.API.Models;
using Bulletin.API.Models.Authentication;
using Bulletin.API.Models.Server;
using Bulletin.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bulletin.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserAccountController : ControllerBase
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly BulletinDBContext _context;
        private readonly IUserAccountService _userAccountService;

        public UserAccountController(
            BulletinDBContext context,
            ILogger<UserAccountController> logger,
            IUserAccountService userAccountService)
        {
            _logger = logger;
            _context = context;
            _userAccountService = userAccountService;
        }

        [HttpGet("all")]
        public async Task<IEnumerable<UserAccount>> GetUsers() =>
            await _context.UserAccounts.ToListAsync();

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile([FromQuery] string username)
        {
            try
            {
                UserAccount user = await _userAccountService.GetUserByUsername(username);
                (int followingCount, int followerCount) = await _userAccountService.GetFollowInfo(username);
                bool isFollowing = await _userAccountService.UserIsFollowing(username);

                return Ok(new
                {
                    User = user,
                    followingCount,
                    followerCount,
                    isFollowing
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("follow")]
        [Authorize]
        public async Task<IActionResult> FollowUser([FromQuery] string username)
        {
            try
            {
                await _userAccountService.FollowUser(username);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("unfollow")]
        [Authorize]
        public async Task<IActionResult> UnfollowUser([FromQuery] string username)
        {
            try
            {
                await _userAccountService.UnfollowUser(username);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromForm] UserAccount formAccount)
        {
            try
            {
                AuthenticationResponse authResponse = await _userAccountService.CreateAccount(formAccount);
                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] UserAccount formAccount)
        {
            try
            {
                AuthenticationResponse authResponse = await _userAccountService.Login(formAccount);
                return Ok(authResponse);
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
