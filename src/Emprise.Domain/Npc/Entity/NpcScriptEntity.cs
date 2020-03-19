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
        /// 脚本名称
        /// </summary>
        public string Name { set; get; }


        public string ActionName { set; get; }
        

        /// <summary>
        /// 默认对话
        /// </summary>
        public string InitWords { set; get; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { set; get; }
    }
}
