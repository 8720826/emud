using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class UnLoadWareCommand : Command
    {
        public int PlayerId { get; set; }

        public int MyWareId { get; set; }
        public UnLoadWareCommand(int playerId, int myWareId)
        {
            PlayerId = playerId;
            MyWareId = myWareId;
        }
    }
}
