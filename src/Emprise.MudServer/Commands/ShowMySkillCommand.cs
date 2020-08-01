using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class ShowMySkillCommand : Command
    {

        public int PlayerId { get; set; }


        public ShowMySkillCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
