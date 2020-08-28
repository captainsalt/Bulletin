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

        public int Code { get; set; }

        public Error(string message, int code)
        {
            Message = message;
            Code = code;
        }
    }
}
