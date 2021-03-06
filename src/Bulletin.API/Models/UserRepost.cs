﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Bulletin.API.Models
{
    public class UserRepost : Entity
    {
        [NotMapped]
        public new int ID { get; set; }

        public int UserId { get; set; }
        public UserAccount User { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
