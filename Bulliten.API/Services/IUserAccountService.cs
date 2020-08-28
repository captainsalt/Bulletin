using Bulliten.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bulliten.API.Services
{
    public interface IUserAccountService
    {
        Task<AuthenticationResponse> Authenticate(AuthenticationRequest model);
    }
}
