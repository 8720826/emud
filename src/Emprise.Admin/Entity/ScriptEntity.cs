using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Entity
{
    public class ScriptEntity : Emprise.Domain.Npc.Entity.ScriptEntity
    {
        public List<NpcScriptEntity> NpcScripts { get; set; }


        public List<ScriptCommandEntity> ScriptCommands { get; set; }
    }
}
