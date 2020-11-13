using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.WareCommands
{
     
    public class BuyStoreWareCommand : Command
    {
        public int PlayerId { get; set; }

        public int StoreWareId { get; set; }

        public int Number { get; set; }

        public BuyStoreWareCommand(int playerId, int id,int number)
        {
            PlayerId = playerId;
            StoreWareId = id;
            Number = number;
        }
    }
}
