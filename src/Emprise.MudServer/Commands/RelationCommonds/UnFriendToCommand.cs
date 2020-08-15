using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.RelationCommonds
{

    public class UnFriendToCommand : Command
    {
        public int PlayerId { get; set; }
        public int RelationId { get; set; }
        public UnFriendToCommand(int playerId, int relationId)
        {
            PlayerId = playerId;
            RelationId = relationId;
        }
    }
}
