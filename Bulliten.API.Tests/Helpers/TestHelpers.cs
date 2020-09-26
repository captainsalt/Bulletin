using Bulliten.API.Middleware;
using Bulliten.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using System;
using System.Collections.Generic;

namespace Bulliten.API.Tests.Helpers
{
    public static class TestHelpers
    {
        public static void Setup(this BullitenDBContext context, Action<BullitenDBContext> action)
        {
            action.Invoke(context);
            context.SaveChanges();
        }

        public static void SetContextUser(this IHttpContextAccessor accessor, UserAccount user) => 
            accessor.HttpContext.Items[JwtMiddleware.CONTEXT_USER] = user;

        public static IEnumerable<UserAccount> GenerateUserAccounts(int count)
        {
            var list = new List<UserAccount>();

            for (int i = 1; i <= count; i++)
            {
                string username = $"User{i}";
                list.Add(new UserAccount { Username = username, Password = "test" });
            }

            return list;
        }
    }
}
