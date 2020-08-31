using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bulliten.API.Models
{
    public class Post
    {
        public int ID { get; set; }

        public UserAccount Author { get; set; }

        public string Content { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
