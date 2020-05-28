using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.User.Models
{
    public class UserLoginDto
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
