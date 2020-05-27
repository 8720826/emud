using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class ModifyPasswordCommand : Command
    {
        public int UserId { get; set; }

        public string Password { get; set; }

        public string NewPassword { get; set; }


        public ModifyPasswordCommand(int userId, string password, string newPassword)
        {
            UserId = userId;
            Password = password;
            NewPassword = newPassword;
        }

    }
}
