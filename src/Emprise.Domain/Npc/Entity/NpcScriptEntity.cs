using Emprise.Domain.Core.Entity;
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



        public int ScriptId { set; get; }


    }
}
