using Bulliten.API.Models;
using Bulliten.API.Models.Authentication;
using Bulliten.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Threading.Tasks;

namespace Bulliten.API.Tests.Helpers
{
    public class GlobalMocks
    {
        public GlobalMocks()
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock
                .Setup(m => m.HttpContext)
                .Returns(new DefaultHttpContext());

            HttpContextAccessor = httpContextAccessorMock.Object;

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(m => m.Authenticate(It.IsAny<AuthenticationRequest>()))
                .Returns<AuthenticationRequest>(req =>
                {
                    var userAccount = new UserAccount { Username = req.Username };
                    return Task.FromResult(new AuthenticationResponse(userAccount, token: string.Empty));
                });

            AuthenticationService = authServiceMock.Object;
        }

        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public IAuthenticationService AuthenticationService { get; set; }
    }
}
