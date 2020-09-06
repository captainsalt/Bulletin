using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bulliten.API.Models
{
    interface IEntity
    {
        public DateTime CreationDate { get; set; }
    }
}
