using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{

    public class PlayerActionCommand : Command
    {
        public int TargetId { get; set; }

        public string CommandName { get; set; }

        public int PlayerId { get; set; }




        public PlayerActionCommand(int playerId, int targetId, string commandName)
        {
            TargetId = targetId;
            PlayerId = playerId;
            CommandName = commandName;
        }

    }
}
