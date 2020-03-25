using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.NpcScript
{
    public class ScriptCommandInput
    {
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
        [Required(ErrorMessage = "分支名称 是必填项")]

        [StringLength(100, ErrorMessage = "分支名称 长度不能超过20")]
        public string Name { set; get; }

        [Required(ErrorMessage = "描述 是必填项")]

        [StringLength(100,ErrorMessage = "描述 长度不能超过100")]
        public string Description { set; get; }

        public string CaseIf { set; get; }


        public string CaseThen { set; get; }

        public string CaseElse { set; get; }

    }
}
