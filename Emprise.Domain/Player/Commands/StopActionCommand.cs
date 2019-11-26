using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Player.Commands
{

    public class StopActionCommand : Command
    {

        public int PlayerId { get; set; }


        public StopActionCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
