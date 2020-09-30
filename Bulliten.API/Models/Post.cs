using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Bulliten.API.Models
{
    public class Post : Entity
    {
        public string Content { get; set; }

        public int Likes { get; set; }

        public int RePosts { get; set; }

        public int AuthorId { get; set; }

        public UserAccount Author { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserLike> LikedBy { get; set; } = new List<UserLike>();

        [JsonIgnore]
        public virtual ICollection<UserRepost> RepostedBy { get; set; } = new List<UserRepost>();

        /// <summary>
        /// True if post is reposted by the context user
        /// </summary>
        [NotMapped]
        public bool RePostStatus { get; set; }

        /// <summary>
        /// True if post is liked by the context user
        /// </summary>
        [NotMapped]
        public bool LikeStatus { get; set; }
    }
}
