using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.WareCommands
{

    public class ShowStoreWareCommand : Command
    {
        public int PlayerId { get; set; }

        public int StoreWareId { get; set; }
        public ShowStoreWareCommand(int playerId, int id)
        {
            PlayerId = playerId;
            StoreWareId = id;
        }
    }
}
