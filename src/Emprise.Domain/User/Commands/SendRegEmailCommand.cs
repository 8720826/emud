using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.User.Commands
{
    public class SendRegEmailCommand : Command
    {

        public string Email { get; set; }


        public SendRegEmailCommand(string email)
        {
            Email = email;
        }

    }
}
