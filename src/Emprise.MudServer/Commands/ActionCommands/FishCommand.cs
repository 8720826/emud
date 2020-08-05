using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class FishCommand : Command
    {

        public int PlayerId { get; set; }


        public FishCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
