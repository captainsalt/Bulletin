using Bulletin.API.Models;
using Bulletin.API.Models.Authentication;
using System.Threading.Tasks;

namespace Bulletin.API.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> Authenticate(AuthenticationRequest request);

        Task<bool> CheckPassword(UserAccount user, string password);

        Task<string> HashPassword(string password);
    }
}
