using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class CollectCommand : Command
    {

        public int PlayerId { get; set; }


        public CollectCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
