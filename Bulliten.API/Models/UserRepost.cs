﻿namespace Bulliten.API.Models
{
    public class UserRepost
    {
        public int UserId { get; set; }
        public UserAccount User { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
