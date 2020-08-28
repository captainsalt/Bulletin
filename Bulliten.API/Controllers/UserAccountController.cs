using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Bulliten.API.Models;
using Bulliten.API.Services;
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromForm] UserAccount formAccount)
        {
            var isInvalidUsername = await _context.UserAccounts.AnyAsync(u => u.Username == formAccount.Username);

            if (isInvalidUsername)
                return BadRequest(new Error($"Username \"{formAccount.Username}\" is already in use", 400));

            await _context.UserAccounts.AddAsync(formAccount);
            await _context.SaveChangesAsync();

            var auth = await _authService.Authenticate(new AuthenticationRequest { Username = formAccount.Username, Password = formAccount.Password });

            return StatusCode(201, auth.Token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] UserAccount formAccount)
        {
            var matchedAccount = await _context.UserAccounts.SingleOrDefaultAsync(u => u.Username == formAccount.Username && u.Password == formAccount.Password);

            if (matchedAccount == null)
                return BadRequest(new Error("Invalid username or password", 400));

            var auth = await _authService.Authenticate(new AuthenticationRequest { Username = formAccount.Username, Password = formAccount.Password });
            return Ok(new { token = auth.Token });
        }
    }
}
