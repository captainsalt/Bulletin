using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bulliten.API.Models
{
    public class UserXUser
    {
        public int FolloweeId { get; set; }
        public UserAccount Followee { get; set; }

        public int FollowerId { get; set; }
        public UserAccount Follower { get; set; }
    }
}
