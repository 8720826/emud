using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.WareCommands
{
     
    public class ShowShopCommand : Command
    {
        public int PlayerId { get; set; }

        public ShowShopCommand(int playerId)
        {
            PlayerId = playerId;
        }
    }
}
