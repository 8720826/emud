using Emprise.Domain.Core.Attributes;
using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Application.Ware.Dtos
{
    public class WareInput
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(20, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string Name { set; get; }

        /// <summary>
        /// 类型，一级分类
        /// </summary>
        [Display(Name = "大类")]
        [EnumValidation(typeof(WareCategoryEnum), ErrorMessage = "请选择{0}")]
        public WareCategoryEnum Category { set; get; }

        /// <summary>
        /// 类型，二级分类
        /// </summary>
        [Display(Name = "小类")]
        [EnumValidation(typeof(WareTypeEnum), ErrorMessage = "请选择{0}")]
        public WareTypeEnum Type { set; get; }


        /// <summary>
        /// 效果，使用后产生，使用后物品会被消耗
        /// </summary>
        [Display(Name = "效果")]
        [StringLength(4000, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string Effect { set; get; }



        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(500, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string Description { set; get; }


        /// <summary>
        /// 图片
        /// </summary>
        [Display(Name = "图片")]
        [Required(ErrorMessage = "请上传{0}")]
        [StringLength(500, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string Img { set; get; }
    }
}
