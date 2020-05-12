using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.Ware
{
    public class WareInput
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        [Required(ErrorMessage = "请填写物品名称")]
        [StringLength(20, ErrorMessage = "最长不能超过20个字符")]
        public string Name { set; get; }

        /// <summary>
        /// 类型，一级分类
        /// </summary>
        [Display(Name = "大类")]
        [Required(ErrorMessage = "请选择物品大类")]
        public WareCategoryEnum Category { set; get; }

        /// <summary>
        /// 类型，二级分类
        /// </summary>
        [Display(Name = "小类")]
        public WareTypeEnum Type { set; get; }


        /// <summary>
        /// 效果，使用后产生，使用后物品会被消耗
        /// </summary>
        [Display(Name = "效果")]
        public string Effect { set; get; }



        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        [Required(ErrorMessage = "请填写物品描述")]
        [StringLength(500, ErrorMessage = "最长不能超过500个字符")]
        public string Description { set; get; }


        /// <summary>
        /// 图片
        /// </summary>
        [Display(Name = "图片")]
        [Required(ErrorMessage = "请上传图片")]
        public string Img { set; get; }
    }
}
