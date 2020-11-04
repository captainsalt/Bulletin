using Bulletin.API.Models;
using Bulletin.API.Models.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bulletin.API.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _config;
        private readonly BulletinDBContext _context;

        public AuthenticationService(IConfiguration config, BulletinDBContext context)
        {
            _config = config;
            _context = context;
        }

        public async Task<AuthenticationResponse> Authenticate(AuthenticationRequest request)
        {
            UserAccount user = await _context.UserAccounts
                .SingleOrDefaultAsync(x => x.Username == request.Username);

            // return null if user not found
            if (user == null)
                return null;

            if (await CheckPassword(user, request.Password) != true)
                return null;

            // authentication successful so generate jwt token
            string token = GenerateJwtToken(user);

            return new AuthenticationResponse(user, token);
        }

        // https://stackoverflow.com/questions/4181198/how-to-hash-a-password/10402129#10402129
        public async Task<string> HashPassword(string password)
        {
            return await Task.Run(() =>
            {
                byte[] salt = new byte[16];
                new RNGCryptoServiceProvider().GetBytes(salt);

                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10_000);
                byte[] hash = pbkdf2.GetBytes(20);

                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);

                string hashedPassword = Convert.ToBase64String(hashBytes);

                return hashedPassword;
            });
        }

        // https://stackoverflow.com/questions/4181198/how-to-hash-a-password/10402129#10402129
        public async Task<bool> CheckPassword(UserAccount user, string password)
        {
            return await Task.Run(async () =>
            {
                string savedPasswordHash = (await _context.UserAccounts.SingleOrDefaultAsync(u => u.Username == user.Username)).Password;

                byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);

                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10_000);
                byte[] hash = pbkdf2.GetBytes(20);

                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                        return false;
                }

                return true;
            });
        }

        private string GenerateJwtToken(UserAccount user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_config["Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()) ,
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
