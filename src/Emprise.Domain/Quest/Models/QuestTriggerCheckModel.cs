using Emprise.Domain.Npc.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Quest.Models
{
    public  class QuestTriggerCheckModel
    {
        public int PlayerId { get; set; }
        public int NpcId { get; set; }
    }
}
