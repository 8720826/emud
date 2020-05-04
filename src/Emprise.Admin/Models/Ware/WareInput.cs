using Emprise.Domain.Core.Enum;
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
        [Required(ErrorMessage = "请填写物品名称")]
        public string Name { set; get; }

        /// <summary>
        /// 类型，一级分类
        /// </summary>

        public WareCategoryEnum Category { set; get; }

        /// <summary>
        /// 类型，二级分类
        /// </summary>

        public WareTypeEnum Type { set; get; }


        /// <summary>
        /// 效果，使用后产生，使用后物品会被消耗
        /// </summary>
        public string Effect { set; get; }



        /// <summary>
        /// 描述
        /// </summary>
        [Required(ErrorMessage = "请填写物品描述")]
        public string Description { set; get; }


        /// <summary>
        /// 图片
        /// </summary>
        [Required(ErrorMessage = "请上传图片")]
        public string Img { set; get; }
    }
}
