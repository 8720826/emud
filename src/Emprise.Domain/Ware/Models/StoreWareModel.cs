using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Ware.Models
{
    public class StoreWareModel
    {
        public int Id { get; set; }


        /// <summary>
        /// 名称
        /// </summary>
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
        public WareEffectAttr WareEffect { set; get; }



        /// <summary>
        /// 描述
        /// </summary>
        public string Description { set; get; }


        /// <summary>
        /// 图片
        /// </summary>
        public string Img { set; get; }

        public int StoreWareId { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public int Price { set; get; }

        public string PriceDesc { set; get; }

        /// <summary>
        /// 原价
        /// </summary>
        public int OriginalPrice { set; get; }


        public int Number { set; get; }


        /// <summary>
        /// 是否购买即绑定
        /// </summary>
        public bool IsBind { set; get; }

        public PriceTypeEnum PriceType { set; get; }
    }
}
