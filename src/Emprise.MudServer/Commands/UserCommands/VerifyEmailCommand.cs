using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.UserCommands
{
    public class VerifyEmailCommand : Command
    {

        public string Email { get; set; }

        public string Code { get; set; }

        public VerifyEmailCommand(string email, string code)
        {
            Email = email;
            Code = code;
        }
    }
}
