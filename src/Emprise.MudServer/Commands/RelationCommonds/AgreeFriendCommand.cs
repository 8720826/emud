using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.RelationCommonds
{
    public class AgreeFriendCommand : Command
    {
        public int PlayerId { get; set; }
        public int RelationId { get; set; }
        public AgreeFriendCommand(int playerId,int relationId)
        {
            PlayerId = playerId;
            RelationId = relationId;
        }
    }
}
