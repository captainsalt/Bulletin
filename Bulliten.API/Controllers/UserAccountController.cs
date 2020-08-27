using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Bulliten.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bulliten.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserAccountController : ControllerBase
    {
        private readonly ILogger<UserAccountController> _logger;

        private static List<UserAccount> _accounts = new List<UserAccount>
        {
            new UserAccount { Username = "test", Password = "test" }
        };

        public UserAccountController(ILogger<UserAccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet("all")]
        public IEnumerable<UserAccount> GetUsers()
        {
            return _accounts;
        }

        [HttpPost("create")]
        public IActionResult CreateAccount([FromForm]UserAccount formAccount)
        {
            var isInvalidUsername = _accounts.Any(u => u.Username == formAccount.Username);

            if (isInvalidUsername) 
                return Problem("Username already in use", statusCode: 400);

            _accounts.Add(formAccount);
            return StatusCode(201);
        }
    }
}
