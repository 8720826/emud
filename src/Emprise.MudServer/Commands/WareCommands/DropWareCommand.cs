using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.WareCommands
{

    public class DropWareCommand : Command
    {
        public int PlayerId { get; set; }

        public int MyWareId { get; set; }
        public DropWareCommand(int playerId, int myWareId)
        {
            PlayerId = playerId;
            MyWareId = myWareId;
        }
    }
}
