using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Models
{
    public class PlayerCommandModel
    {
        public PlayerCommandModel(string commandName)
        {
            CommandName = commandName;
        }

        public PlayerCommandModel(string command, string tips)
        {
            CommandName = command;
            Tips = tips;
        }

        public string CommandName { get; set; }

        public string Tips { get; set; }
    }
}
