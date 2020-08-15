using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class InitGameCommand : Command
    {
        public int PlayerId { get; set; }


        public InitGameCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
