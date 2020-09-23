using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Bulliten.API.Models
{
    public abstract class Entity<T> : IEntity<T> 
        where T : Entity<T> 
    {
        public Entity()
        {
            CreationDate = DateTime.Now;
        }

        public int ID { get; set; }

        public DateTime CreationDate { get; set; }

        public bool Equals([AllowNull] T x, [AllowNull] T y) => x.ID == y.ID;

        public int GetHashCode([DisallowNull] T obj) => obj.ID;
    }
}
