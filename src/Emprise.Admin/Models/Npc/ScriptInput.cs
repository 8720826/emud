using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.Script
{
    public class ScriptInput
    {
        /// <summary>
        /// 脚本名称
        /// </summary>
        [Display(Name = "脚本名称")]
        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(50, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string Name { set; get; }

        [Display(Name = "描述")]
        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(4000, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string Description { set; get; }


        /// <summary>
        /// 是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        public bool IsEnable { set; get; }
    }
}
