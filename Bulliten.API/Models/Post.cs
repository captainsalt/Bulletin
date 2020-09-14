﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

        public string Content { get; set; }

        public int Likes { get; set; }

        public int RePosts { get; set; }

        public UserAccount Author { get; set; }

        /// <summary>
        /// True if post is reposted by the context user
        /// </summary>
        [NotMapped]
        public bool RePostStatus => RepostedBy.FirstOrDefault(ul => ul.UserId == Author.ID) != null;

        /// <summary>
        /// True if post is liked by the context user
        /// </summary>
        [NotMapped]
        public bool LikeStatus => LikedBy.FirstOrDefault(ul => ul.UserId == Author.ID) != null;

        [JsonIgnore]
        public ICollection<UserLike> LikedBy { get; set; } = new List<UserLike>();

        [JsonIgnore]
        public ICollection<UserRepost> RepostedBy { get; set; } = new List<UserRepost>();
    }
}
