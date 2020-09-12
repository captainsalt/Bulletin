using Bulliten.API.Models.Authentication;
using System.Threading.Tasks;

namespace Bulliten.API.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> Authenticate(AuthenticationRequest model);
    }
}
