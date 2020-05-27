using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class SendResetEmailCommand : Command
    {

        public string Email { get; set; }

        public SendResetEmailCommand(string email)
        {
            Email = email;
        }
    }
}
