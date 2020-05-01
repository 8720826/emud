using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Entity
{
    public class ScriptCommandEntity : Emprise.Domain.Npc.Entity.ScriptCommandEntity
    {
        public ScriptEntity Script { get; set; }
    }
}
