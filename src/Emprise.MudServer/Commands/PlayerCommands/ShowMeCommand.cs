using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class ShowMeCommand : Command
    {

        public int PlayerId { get; set; }


        public ShowMeCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
