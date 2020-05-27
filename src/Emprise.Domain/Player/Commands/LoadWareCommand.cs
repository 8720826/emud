using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Player.Commands
{
    public class LoadWareCommand : Command
    {
        public int PlayerId { get; set; }

        public int WareId { get; set; }
        public LoadWareCommand(int playerId, int wareId)
        {
            PlayerId = playerId;
            WareId = wareId;
        }
    }
}
