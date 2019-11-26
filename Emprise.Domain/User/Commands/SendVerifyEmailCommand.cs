using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.User.Commands
{
    public class SendVerifyEmailCommand : Command
    {

        public string Email { get; set; }


        public SendVerifyEmailCommand(string email)
        {
            Email = email;
        }

    }
}
