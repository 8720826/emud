using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Domain.Ware.Entity
{
    [Table("Ware")]
    public class WareEntity : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(20)]
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
        [StringLength(500)]
        public string Description { set; get; }


        /// <summary>
        /// 图片
        /// </summary>
        [StringLength(500)]
        public string Img { set; get; }

    }
}
