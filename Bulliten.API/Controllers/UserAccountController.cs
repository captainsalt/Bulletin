using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using Bulliten.API.Services;
using Bulliten.API.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bulliten.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserAccountController : ControllerBase
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly BullitenDBContext _context;
        private readonly IAuthenticationService _authService;
        private readonly IUserAccountService _userAccountService;

        public UserAccountController(
            BullitenDBContext context,
            ILogger<UserAccountController> logger,
            IAuthenticationService authService,
            IUserAccountService userAccountService)
        {
            _logger = logger;
            _context = context;
            _authService = authService;
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
                bool isFollowing = await _userAccountService.UserIsFollowing(GetAccountFromContext(), username);

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
                await _userAccountService.FollowUser(GetAccountFromContext(), username);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("unfollow")]
        [Authorize]
        public async Task<IActionResult> UnfollowUser([FromQuery] string username)
        {
            try
            {
                await _userAccountService.UnfollowUser(GetAccountFromContext(), username);
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

        private UserAccount GetAccountFromContext() =>
            (UserAccount)HttpContext.Items[JwtMiddleware.CONTEXT_USER];

        private IActionResult HandleException(Exception ex)
        {
            if (ex is ArgumentException argEx)
            {
                return BadRequest(new Error(argEx.Message));
            }
            else
            {
                _logger.LogCritical(ex, "Internal server error");
                return Problem("An internal server error has occured");
            }
        }
    }
}
