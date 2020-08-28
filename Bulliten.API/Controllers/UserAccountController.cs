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
        private readonly IUserAccountService _accountService;

        public UserAccountController(ILogger<UserAccountController> logger, BullitenDBContext context, IUserAccountService accountService)
        {
            _logger = logger;
            _context = context;
            _accountService = accountService;
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
                return BadRequest(new Error($"Username {formAccount.Username} is already in use", 400));

            await _context.UserAccounts.AddAsync(formAccount);
            await _context.SaveChangesAsync();

            var response = await _accountService.Authenticate(new AuthenticationRequest { Username = formAccount.Username, Password = formAccount.Password });

            return StatusCode(201, response.Token);
        }
    }
}
