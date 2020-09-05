using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bulliten.API.Models.Authentication
{
    public class AuthenticationResponse
    {
        public int ID { get; set; }

        public string Username { get; set; }

        public string Token { get; set; }

        public AuthenticationResponse(UserAccount user, string token)
        {
            ID = user.ID;
            Username = user.Username;
            Token = token;
        }
    }
}
