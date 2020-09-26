using Bulliten.API.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Bulliten.API.Tests.Helpers
{
    public static class BullitenDBContextExtensions
    {
        public static void Setup(this BullitenDBContext context, Action<BullitenDBContext> action)
        {
            action.Invoke(context);
            context.SaveChanges();
        }

        public static IEnumerable<UserAccount> GenerateUserAccounts(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                string username = $"User{i}";
                yield return new UserAccount { Username = username, Password = "test" };
            }
        }
    }
}
