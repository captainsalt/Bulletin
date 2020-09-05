using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bulliten.API.Models
{
    public class Error
    {
        public string Message { get; set; }

        public Error(string message)
        {
            Message = message;
        }
    }
}
