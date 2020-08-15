using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.NpcActionCommands
{

    public class KillNpcCommand : Command
    {
        public int NpcId { get; set; }


        public int PlayerId { get; set; }




        public KillNpcCommand(int playerId, int npcId)
        {
            NpcId = npcId;
            PlayerId = playerId;
        }

    }
}
