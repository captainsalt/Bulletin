using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bulletin.API.Models
{
    public class UserAccount : Entity
    {
        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

        [JsonIgnore]
        public virtual ICollection<FollowRecord> Followers { get; set; } = new List<FollowRecord>();

        [JsonIgnore]
        public virtual ICollection<UserRepost> RePosts { get; set; } = new List<UserRepost>();

        [JsonIgnore]
        public virtual ICollection<UserLike> LikedPosts { get; set; } = new List<UserLike>();
    }
}
