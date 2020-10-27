using Bulletin.API.Models;
using Bulletin.API.Models.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Bulletin.API.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (UserAccount)context.HttpContext.Items[JwtMiddleware.CONTEXT_USER];

            if (user == null)
                context.Result = new UnauthorizedObjectResult(new JsonError("Unauthorized"));
        }
    }
}
