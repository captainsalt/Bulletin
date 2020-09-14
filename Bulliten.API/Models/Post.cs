using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

        public int Likes { get; set; }

        public int RePosts { get; set; }

        [JsonIgnore]
        public ICollection<UserLike> LikedBy { get; set; } = new List<UserLike>();

        [JsonIgnore]
        public ICollection<UserRepost> RepostedBy { get; set; } = new List<UserRepost>();
    }
}
