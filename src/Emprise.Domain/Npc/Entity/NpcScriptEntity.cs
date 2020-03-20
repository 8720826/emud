using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Domain.Npc.Entity
{
    [Table("NpcScript")]
    public class NpcScriptEntity : BaseEntity
    {
        /// <summary>
        /// NpcId
        /// </summary>
        public int NpcId { set; get; }
        public NpcEntity Npc { get; set; }


        public int ScriptId { set; get; }
        public ScriptEntity Script { get; set; }

    }
}
