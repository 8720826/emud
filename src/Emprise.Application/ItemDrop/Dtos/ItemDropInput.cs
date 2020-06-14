using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Application.ItemDrop.Dtos
{
    public class ItemDropInput
    {
        [Display(Name = "名称")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(32, ErrorMessage = "{0}长度最大为{1}字符")]
        public string Name { set; get; }

        /// <summary>
        /// 脚本描述
        /// </summary>
        [Display(Name = "名称")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(500, ErrorMessage = "{0}长度最大为{1}字符")]
        public string Description { set; get; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { set; get; }
    }
}
