using Bulletin.API.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Bulletin.API.Utilities
{
    public class EntityComparer<T> : IEqualityComparer<T> where T : IEntity
    {
        public bool Equals([AllowNull] T x, [AllowNull] T y) => x?.ID == y?.ID;

        public int GetHashCode([DisallowNull] T obj) => obj.ID;
    }

    public static class ExtentionsMethods
    {
        public static IEnumerable<T> NoDuplicates<T>(this IEnumerable<T> entities) where T : IEntity
        {
            var newList = new HashSet<T>(new EntityComparer<T>());

            foreach (T entity in entities)
                newList.Add(entity);

            return newList;
        }
    }
}
