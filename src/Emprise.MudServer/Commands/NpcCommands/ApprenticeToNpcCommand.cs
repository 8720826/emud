using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.NpcCommands
{
    public class ApprenticeToNpcCommand : Command
    {
        public int NpcId { get; set; }


        public int PlayerId { get; set; }




        public ApprenticeToNpcCommand(int playerId, int npcId)
        {
            NpcId = npcId;
            PlayerId = playerId;
        }

    }
}
