using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
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
