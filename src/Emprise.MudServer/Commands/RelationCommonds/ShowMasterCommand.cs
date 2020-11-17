using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.RelationCommonds
{

    public class ShowMasterCommand : Command
    {
        public int PlayerId { get; set; }

        public ShowMasterCommand(int playerId)
        {
            PlayerId = playerId;
        }
    }
}
