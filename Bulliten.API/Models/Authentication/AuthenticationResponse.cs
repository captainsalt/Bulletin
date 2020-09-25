using Microsoft.Extensions.Configuration;

namespace Bulliten.API.Models.Authentication
{
    public class AuthenticationResponse
    {
        public UserAccount User { get; set; }

        public string Token { get; set; }

        public AuthenticationResponse(UserAccount user, string token)
        {
            User = user;
            Token = token;
        }
    }
}
