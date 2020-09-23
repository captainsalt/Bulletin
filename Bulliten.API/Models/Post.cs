using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace Bulliten.API.Models
{
    public class Post : Entity
    {
        public string Content { get; set; }

        public int Likes { get; set; }

        public int RePosts { get; set; }

        public UserAccount Author { get; set; }

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

        [JsonIgnore]
        public ICollection<UserLike> LikedBy { get; set; } = new List<UserLike>();

        [JsonIgnore]
        public ICollection<UserRepost> RepostedBy { get; set; } = new List<UserRepost>();

        public void PopulateStatuses(UserAccount user)
        {
            RePostStatus = RepostedBy.Any(ul => ul.UserId == user.ID);
            LikeStatus = LikedBy.Any(ul => ul.UserId == user.ID);
        }
    }
}
