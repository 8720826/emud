using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class DigCommand : Command
    {

        public int PlayerId { get; set; }


        public DigCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
