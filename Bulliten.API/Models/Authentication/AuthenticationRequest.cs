﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bulliten.API.Models.Authentication
{
    public class AuthenticationRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}