using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Bulliten.API.Models
{
    public class UserAccount : IEntity<UserAccount>
    {
        public UserAccount()
        {
            CreationDate = DateTime.UtcNow;
        }

        public int ID { get; set; }

        public DateTime CreationDate { get; set; }

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

        public bool Equals([AllowNull] UserAccount x, [AllowNull] UserAccount y) => x.ID == y.ID;

        public int GetHashCode([DisallowNull] UserAccount obj) => throw new NotImplementedException();
    }
}
