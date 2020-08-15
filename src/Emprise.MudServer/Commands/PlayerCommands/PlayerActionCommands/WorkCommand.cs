using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class WorkCommand : Command
    {

        public int PlayerId { get; set; }


        public WorkCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
