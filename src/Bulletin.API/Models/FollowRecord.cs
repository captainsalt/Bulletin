namespace Bulletin.API.Models
{
    public class FollowRecord : Entity
    {
        public int FolloweeId { get; set; }
        public UserAccount Followee { get; set; }

        public int FollowerId { get; set; }
        public UserAccount Follower { get; set; }
    }
}
