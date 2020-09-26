using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bulliten.API.Tests.Helpers
{
    public class GlobalMocks
    {
        public GlobalMocks()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m["Secret"]).Returns("SecretTestString");

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock
                .Setup(m => m.HttpContext)
                .Returns(new DefaultHttpContext());

            HttpContextAccessor = httpContextAccessorMock.Object;
            Configuration = configMock.Object;
        }

        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public IConfiguration Configuration { get; set; }
    }
}
