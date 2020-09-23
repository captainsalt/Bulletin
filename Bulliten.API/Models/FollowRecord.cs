namespace Bulliten.API.Models
{
    public class FollowRecord : Entity<FollowRecord>
    {
        public int FolloweeId { get; set; }
        public UserAccount Followee { get; set; }

        public int FollowerId { get; set; }
        public UserAccount Follower { get; set; }
    }
}
