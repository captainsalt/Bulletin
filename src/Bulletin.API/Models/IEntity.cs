using System;

namespace Bulletin.API.Models
{
    public interface IEntity
    {
        public int ID { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
