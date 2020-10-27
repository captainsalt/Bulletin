using Bulletin.API.Middleware;
using Bulletin.API.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Bulletin.API.Tests.Helpers
{
    public static class TestHelpers
    {
        public static void Setup(this BulletinDBContext context, Action<BulletinDBContext> action)
        {
            action.Invoke(context);
            context.SaveChanges();
        }

        public static BulletinDBContext AddUsers(this BulletinDBContext context, int quantity)
        {
            var list = new List<UserAccount>();

            for (int i = 1; i <= quantity; i++)
            {
                string username = $"User{i}";
                list.Add(new UserAccount { Username = username, Password = "test" });
            }

            context.UserAccounts.AddRange(list);
            context.SaveChanges();

            return context;
        }

        public static BulletinDBContext AddUsers(this BulletinDBContext context, params UserAccount[] users)
        {
            context.UserAccounts.AddRange(users);
            context.SaveChanges();

            return context;
        }

        public static IEnumerable<Post> GenerateRandomPosts(int quantity)
        {
            var list = new List<Post>();

            for (int i = 1; i <= quantity; i++)
                list.Add(new Post() { });

            return list;
        }

        public static UserAccount GetUserById(this BulletinDBContext context, int id) =>
            context.UserAccounts.Find(id);

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
