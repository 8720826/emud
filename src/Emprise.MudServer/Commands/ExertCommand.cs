using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class ExertCommand : Command
    {

        public int PlayerId { get; set; }


        public ExertCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
