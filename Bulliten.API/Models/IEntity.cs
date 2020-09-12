using System;

namespace Bulliten.API.Models
{
    internal interface IEntity
    {
        public DateTime CreationDate { get; set; }
    }
}
