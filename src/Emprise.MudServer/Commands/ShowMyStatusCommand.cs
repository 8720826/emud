using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class ShowMyStatusCommand : Command
    {

        public int PlayerId { get; set; }


        public ShowMyStatusCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
