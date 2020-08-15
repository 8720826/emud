using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class LogoutCommand : Command
    {
        public int Id { get; set; }


        public LogoutCommand(int id)
        {
            Id = id;
        }

    }
}
