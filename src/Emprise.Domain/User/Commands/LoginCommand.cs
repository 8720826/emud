using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.User.Commands
{
    public class LoginCommand : Command
    {
        public string Email { get; set; }

        public string Password { get; set; }


        public LoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }

    }
}
