using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.ItemDrop.Entity
{
    [Table("ItemDropRate")]
    public class ItemDropRateEntity : BaseEntity
    {
        public int ItemDropId { get; set; }

        public ItemDropTypeEnum DropType { set; get; }

        public int WareId { set; get; }  

        public int MaxNumber { set; get; }


        public int MinNumber { set; get; }

        /// <summary>
        /// 掉落顺序，从小到大
        /// </summary>
        public int Order { set; get; }

        /// <summary>
        /// 权重，总权重为100，掉落物品累加，达到100后不再掉落
        /// </summary>
        public int Weight { set; get; }

        /// <summary>
        /// 奖励几率，掉落百分比
        /// </summary>
        public int Percent { set; get; }
    }
}
