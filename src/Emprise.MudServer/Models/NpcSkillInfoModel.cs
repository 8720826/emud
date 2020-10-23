using Emprise.Domain.Player.Models;
using Emprise.Domain.Skill.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Models
{
    public class NpcSkillInfoModel
    {
        public NpcBaseInfo Npc { get; set; }

        public List<FriendSkillModel> Skills { get; set; }
    }
}
