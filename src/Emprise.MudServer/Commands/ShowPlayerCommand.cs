using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class ShowPlayerCommand : Command
    {
        public int MyId { get; set; }
        public int PlayerId { get; set; }


        public ShowPlayerCommand(int myId, int playerId)
        {
            MyId = myId; 
            PlayerId = playerId;
        }
    }
}
