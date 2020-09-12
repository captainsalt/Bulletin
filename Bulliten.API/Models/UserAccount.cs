﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bulliten.API.Models
{
    public class UserAccount : IEntity
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
    }
}
