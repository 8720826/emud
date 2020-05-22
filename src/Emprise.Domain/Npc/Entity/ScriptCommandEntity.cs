using Emprise.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Domain.Npc.Entity
{
    [Table("ScriptCommand")]
    public class ScriptCommandEntity : BaseEntity
    {
        public int ScriptId { set; get; }

        /// <summary>
        /// 是否入口，入口将显示在Npc资料页
        /// </summary>
        public bool IsEntry { set; get; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortId { set; get; }

        /// <summary>
        /// 分支名称
        /// </summary>
        [StringLength(100)]
        public string Name { set; get; }



        public string CaseIf { set; get; }


        public string CaseThen { set; get; }

        public string CaseElse { set; get; }



    }
}
