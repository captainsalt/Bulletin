using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Bulliten.API.Models
{
    public abstract class Entity : IEntity
    {
        public Entity()
        {
            CreationDate = DateTime.Now;
        }

        public int ID { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
