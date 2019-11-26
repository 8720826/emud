using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Player.Commands
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
