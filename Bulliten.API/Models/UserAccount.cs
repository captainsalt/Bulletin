﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bulliten.API.Models
{
    public class UserAccount
    {
        public int ID { get; set; }

        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }        
    }
}
