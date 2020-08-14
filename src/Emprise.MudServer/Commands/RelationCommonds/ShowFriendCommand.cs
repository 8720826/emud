using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.RelationCommonds
{

    public class ShowFriendCommand : Command
    {
        public int PlayerId { get; set; }

        public ShowFriendCommand(int playerId)
        {
            PlayerId = playerId;
        }
    }
}
