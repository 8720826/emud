using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.SkillCommands
{
    
    public class ShowFightingSkillCommand : Command
    {

        public int PlayerId { get; set; }


        public ShowFightingSkillCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
