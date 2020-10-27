using Bulletin.API.Models.Authentication;
using System.Threading.Tasks;

namespace Bulletin.API.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> Authenticate(AuthenticationRequest model);
    }
}
