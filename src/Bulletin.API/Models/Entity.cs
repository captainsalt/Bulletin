using System;

namespace Bulletin.API.Models
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
