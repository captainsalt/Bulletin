using System;

namespace Bulliten.API.Models
{
    public class Post : IEntity
    {
        public Post()
        {
            CreationDate = DateTime.UtcNow;
        }

        public int ID { get; set; }

        public DateTime CreationDate { get; set; }

        public UserAccount Author { get; set; }

        public string Content { get; set; }
    }
}
