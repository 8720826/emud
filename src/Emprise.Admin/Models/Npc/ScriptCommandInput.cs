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
        [Display(Name = "是否入口")]
        public bool IsEntry { set; get; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int SortId { set; get; }

        /// <summary>
        /// 分支名称
        /// </summary>
        [Display(Name = "分支名称")]
        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(50, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string Name { set; get; }


        [Display(Name = "描述")]
        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(500, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string Description { set; get; }

        [Display(Name = "分支-如果")]
        [StringLength(4000, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string CaseIf { set; get; }

        [Display(Name = "分支-那么")]
        [StringLength(4000, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string CaseThen { set; get; }

        [Display(Name = "分支-否则")]
        [StringLength(4000, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string CaseElse { set; get; }

    }
}
