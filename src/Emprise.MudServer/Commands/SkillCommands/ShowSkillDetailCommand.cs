using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.SkillCommands
{

    public class ShowSkillDetailCommand : Command
    {
        public int PlayerId { get; set; }

        public int MySkillId { get; set; }

        public int Type { get; set; }
        public ShowSkillDetailCommand(int playerId, int mySkillId, int type)
        {
            PlayerId = playerId;
            MySkillId = mySkillId;
            Type = type;
        }
    }
}
