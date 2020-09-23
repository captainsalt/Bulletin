using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Bulliten.API.Models
{
    public class UserAccount : Entity<UserAccount>
    {
        public UserAccount()
        {
            CreationDate = DateTime.UtcNow;
        }

        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public ICollection<Post> Posts { get; set; } = new List<Post>();

        [JsonIgnore]
        public ICollection<FollowRecord> Followers { get; set; } = new List<FollowRecord>();

        [JsonIgnore]
        public ICollection<UserRepost> RePosts { get; set; } = new List<UserRepost>();

        [JsonIgnore]
        public ICollection<UserLike> LikedPosts { get; set; } = new List<UserLike>();
    }
}
