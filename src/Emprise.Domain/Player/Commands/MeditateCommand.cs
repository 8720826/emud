using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Player.Commands
{
    public class MeditateCommand : Command
    {

        public int PlayerId { get; set; }


        public MeditateCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
