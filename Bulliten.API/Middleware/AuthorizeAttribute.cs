using Bulliten.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Bulliten.API.Middleware
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
