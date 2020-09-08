using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using Bulliten.API.Services;
using Bulliten.API.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bulliten.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserAccountController : ControllerBase
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly BullitenDBContext _context;
        private readonly IAuthenticationService _authService;

        public UserAccountController(ILogger<UserAccountController> logger, BullitenDBContext context, IAuthenticationService accountService)
        {
            _logger = logger;
            _context = context;
            _authService = accountService;
        }

        [HttpGet("all")]
        public async Task<IEnumerable<UserAccount>> GetUsers()
        {
            return await _context.UserAccounts.ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserByUsername([FromQuery] string username)
        {
            var user = await _context.UserAccounts.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return BadRequest(new Error("User does not exit"));

            return Ok(new { user });
        }

        [HttpGet("followinfo")]
        [Authorize]
        public async Task<IActionResult> GetUserFollowings([FromQuery] string username)
        {
            var user = await _context.UserAccounts
                .Include(u => u.Followers)
                .SingleOrDefaultAsync(u => u.Username == username);

            var following = await _context.FollowerTable.Where(uxu => uxu.FollowerId == user.ID).CountAsync();

            if (user == null)
                return BadRequest(new Error("User does not exit"));

            return Ok(new { followers = user.Followers.Count, following });
        }

        [HttpPost("follow")]
        [Authorize]
        public async Task<IActionResult> FollowUser([FromQuery] string username)
        {
            var ctxUser = GetAccountFromContext();

            if (ctxUser.Username == username)
                return BadRequest(new Error("Cannot follow yourself"));

            var targetUser = await _context.UserAccounts
               .Include(u => u.Followers)
               .SingleOrDefaultAsync(u => u.Username == username);

            if (targetUser == null)
                return BadRequest(new Error("User does not exist"));

            var followRecord = targetUser.Followers
                .SingleOrDefault(uxu => uxu.FollowerId == ctxUser.ID);

            if (followRecord != null)
                return BadRequest(new Error("Already following"));

            targetUser.Followers.Add(new UserXUser
            {
                Followee = targetUser,
                Follower = ctxUser,
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromForm] UserAccount formAccount)
        {
            var isInvalidUsername = await _context.UserAccounts.AnyAsync(u => u.Username == formAccount.Username);

            if (isInvalidUsername)
                return BadRequest(new Error($"Username \"{formAccount.Username}\" is already in use"));

            await _context.UserAccounts.AddAsync(formAccount);
            await _context.SaveChangesAsync();

            var auth = await _authService.Authenticate(new AuthenticationRequest { Username = formAccount.Username, Password = formAccount.Password });

            return StatusCode(201, new { token = auth.Token, user = formAccount });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] UserAccount formAccount)
        {
            var matchedAccount = await _context.UserAccounts.SingleOrDefaultAsync(u => u.Username == formAccount.Username && u.Password == formAccount.Password);

            if (matchedAccount == null)
                return BadRequest(new Error("Invalid username or password"));

            var auth = await _authService.Authenticate(new AuthenticationRequest { Username = matchedAccount.Username, Password = matchedAccount.Password });
            return Ok(new { token = auth.Token, user = matchedAccount });
        }

        private UserAccount GetAccountFromContext() =>
           (UserAccount)HttpContext.Items[JwtMiddleware.CONTEXT_USER];
    }
}
