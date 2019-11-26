using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.User.Commands
{

    public class ResetPasswordCommand : Command
    {

        public string Email { get; set; }



        public ResetPasswordCommand(string email)
        {
            Email = email;
        }

    }
}
