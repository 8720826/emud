using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Domain.Npc.Entity
{
    [Table("Script")]
    public class ScriptEntity : BaseEntity
    {
        /// <summary>
        /// 脚本名称
        /// </summary>
        [StringLength(100)]
        public string Name { set; get; }

        [StringLength(100)]
        public string ActionName { set; get; }
        

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { set; get; }


        public List<NpcScriptEntity> NpcScripts { get; set; }


        public List<ScriptCommandEntity> ScriptCommands { get; set; }
    }
}
