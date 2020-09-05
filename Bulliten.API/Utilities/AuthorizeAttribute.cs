using Bulliten.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bulliten.API.Utilities
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (UserAccount)context.HttpContext.Items[JwtMiddleware.CONTEXT_USER];

            if (user == null)
                context.Result = new BadRequestObjectResult(new Error("Unauthorized"));
        }
    }
}
