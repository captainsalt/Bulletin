using System.ComponentModel.DataAnnotations.Schema;

namespace Bulliten.API.Models
{
    public class UserLike : Entity
    {
        [NotMapped]
        public new int ID { get; set; }

        public int UserId { get; set; }
        public UserAccount User { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
