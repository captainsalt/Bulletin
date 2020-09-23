using System;

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
