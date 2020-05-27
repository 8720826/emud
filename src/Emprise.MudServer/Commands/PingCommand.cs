using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class PingCommand : Command
    {
        public int PlayerId { get; set; }


        public PingCommand(int playerId)
        {
            PlayerId = playerId;
        }
    }
}
