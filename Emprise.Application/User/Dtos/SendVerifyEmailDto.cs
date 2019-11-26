using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Application.User.Dtos
{
    public class SendVerifyEmailDto
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
