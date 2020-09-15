using System;
using System.Collections.Generic;

namespace Bulliten.API.Models
{
    public interface IEntity<T> : IEqualityComparer<T>
    {
        public int ID { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
