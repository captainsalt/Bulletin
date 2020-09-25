using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using Bulliten.API.Services;
using Bulliten.API.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> GetUserByUsername([FromQuery] string username)
        {
            UserAccount user = await _context.UserAccounts
                .Include(u => u.Followers)
                .SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return BadRequest(new Error("User does not exist"));

            int followerCount = await _context.FollowerTable
                .Where(fr => fr.FolloweeId == user.ID)
                .CountAsync();

            int followingCount = await _context.FollowerTable
                .Where(fr => fr.FollowerId == user.ID)
                .CountAsync();

            UserAccount contextUser = GetAccountFromContext();

            bool isFollowed = user.Followers.Any(fr => fr.FollowerId == contextUser.ID);

            return Ok(new { user, followerCount, followingCount, isFollowed });
        }

        [HttpPost("follow")]
        [Authorize]
        public async Task<IActionResult> FollowUser([FromQuery] string username)
        {
            UserAccount ctxUser = GetAccountFromContext();

            if (ctxUser.Username == username)
                return BadRequest(new Error("Cannot follow yourself"));

            UserAccount targetUser = await _context.UserAccounts
                .Include(u => u.Followers)
                .SingleOrDefaultAsync(u => u.Username == username);

            if (targetUser == null)
                return BadRequest(new Error("User does not exist"));

            FollowRecord followRecord = targetUser.Followers
                .SingleOrDefault(fr => fr.FollowerId == ctxUser.ID);

            if (followRecord != null)
                return BadRequest(new Error("Already following"));

            targetUser.Followers.Add(new FollowRecord
            {
                Followee = targetUser,
                Follower = ctxUser,
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("unfollow")]
        [Authorize]
        public async Task<IActionResult> UnfollowUser([FromQuery] string username)
        {
            UserAccount ctxUser = GetAccountFromContext();

            if (ctxUser.Username == username)
                return BadRequest(new Error("Cannot unfollow yourself"));

            UserAccount targetUser = await _context.UserAccounts
                .Include(u => u.Followers)
                .SingleOrDefaultAsync(u => u.Username == username);

            if (targetUser == null)
                return BadRequest(new Error("User does not exist"));

            FollowRecord followRecord = targetUser.Followers
                .SingleOrDefault(fr => fr.FollowerId == ctxUser.ID);

            if (followRecord == null)
                return Ok();

            targetUser.Followers.Remove(followRecord);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromForm] UserAccount formAccount)
        {
            try
            {
                await _userAccountService.CreateAccount(formAccount);
                return Ok();
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
               var authResponse =  await _userAccountService.Login(formAccount);
                return Ok(new { token = authResponse.Token, user = authResponse.User } );
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
