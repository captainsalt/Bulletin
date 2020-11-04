using Bulletin.API.Models;
using Bulletin.API.Models.Authentication;
using Bulletin.API.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Threading.Tasks;

namespace Bulletin.API.Tests.Helpers
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

            authServiceMock
                .Setup(m => m.CheckPassword(It.IsAny<UserAccount>(), It.IsAny<string>()))
                .Returns<UserAccount, string>((account, password) =>
                {
                    if (account?.Password == password)
                        return Task.FromResult(true);
                    else
                        return Task.FromResult(false);
                });

            authServiceMock
                .Setup(m => m.HashPassword(It.IsAny<string>()))
                .Returns<string>(password => Task.FromResult(password));

            AuthenticationService = authServiceMock.Object;
        }

        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public IAuthenticationService AuthenticationService { get; set; }
    }
}
