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
            var user = (UserAccount)context.HttpContext.Items["User"];
            if (user == null)
            {
                context.Result = new JsonResult(new Error("Unauthorized", 401))
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }
    }
}
