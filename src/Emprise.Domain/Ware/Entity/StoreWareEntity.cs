using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Ware.Entity
{

    [Table("StoreWare")]
    public class StoreWareEntity : BaseEntity
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public int WareId { set; get; }


        /// <summary>
        /// 价格
        /// </summary>
        public int Price { set; get; }

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
