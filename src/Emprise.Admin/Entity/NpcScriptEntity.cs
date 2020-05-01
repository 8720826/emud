using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Entity
{
    public class NpcScriptEntity : Emprise.Domain.Npc.Entity.NpcScriptEntity
    {
        public NpcEntity Npc { get; set; }

        public ScriptEntity Script { get; set; }
    }
}
