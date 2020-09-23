using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using Bulliten.API.Services;
using Bulliten.API.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        public UserAccountController(
            BullitenDBContext context,
            ILogger<UserAccountController> logger,
            IAuthenticationService accountService)
        {
            _logger = logger;
            _context = context;
            _authService = accountService;
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

        [HttpGet("followinfo")]
        [Authorize]
        public async Task<IActionResult> GetUserFollowings([FromQuery] string username)
        {
            UserAccount user = await _context.UserAccounts
                .Include(u => u.Followers)
                .SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return BadRequest(new Error("User does not exist"));

            IEnumerable<UserAccount> followers = await _context.FollowerTable
                .Where(fr => fr.FolloweeId == user.ID)
                .Select(fr => fr.Follower)
                .ToListAsync();

            IEnumerable<UserAccount> following = await _context.FollowerTable
                .Where(fr => fr.FollowerId == user.ID)
                .Select(fr => fr.Followee)
                .ToListAsync();

            return Ok(new { following, followers });
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
            bool isInvalidUsername = await _context.UserAccounts.AnyAsync(u => u.Username == formAccount.Username);

            if (isInvalidUsername)
                return BadRequest(new Error($"Username \"{formAccount.Username}\" is already in use"));

            await _context.UserAccounts.AddAsync(formAccount);
            await _context.SaveChangesAsync();

            AuthenticationResponse auth = await _authService
                .Authenticate(new AuthenticationRequest
                {
                    Username = formAccount.Username,
                    Password = formAccount.Password
                });

            return StatusCode(201, new { token = auth.Token, user = formAccount });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] UserAccount formAccount)
        {
            UserAccount matchedAccount = await _context.UserAccounts
                .SingleOrDefaultAsync(u => u.Username == formAccount.Username && u.Password == formAccount.Password);

            if (matchedAccount == null)
                return BadRequest(new Error("Invalid username or password"));

            AuthenticationResponse auth = await _authService
                .Authenticate(new AuthenticationRequest
                {
                    Username = matchedAccount.Username,
                    Password = matchedAccount.Password
                });

            return Ok(new { token = auth.Token, user = matchedAccount });
        }

        private UserAccount GetAccountFromContext() =>
            (UserAccount)HttpContext.Items[JwtMiddleware.CONTEXT_USER];
    }
}
