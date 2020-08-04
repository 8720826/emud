using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.EmailCommands
{

    public class ShowEmailDetailCommand : Command
    {
        public int PlayerId { get; set; }
        public int PlayerEmailId { get; set; }

        public ShowEmailDetailCommand(int playerId, int playerEmailId)
        {
            PlayerId = playerId;
            PlayerEmailId = playerEmailId;
        }
    }
}
