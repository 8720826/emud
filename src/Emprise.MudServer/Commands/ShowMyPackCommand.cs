using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class ShowMyPackCommand : Command
    {

        public int PlayerId { get; set; }


        public ShowMyPackCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
