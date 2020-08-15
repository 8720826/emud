using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class HuntCommand : Command
    {

        public int PlayerId { get; set; }


        public HuntCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
