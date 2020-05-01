using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Entity
{
    public class NpcEntity : Emprise.Domain.Npc.Entity.NpcEntity
    {
        /// <summary>
        /// 脚本
        /// </summary>
        public List<NpcScriptEntity> NpcScripts { get; set; }
    }
}
