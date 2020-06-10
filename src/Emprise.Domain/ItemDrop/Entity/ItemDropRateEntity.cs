using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Room.Entity
{
    [Table("ItemDropRate")]
    public class ItemDropRateEntity : BaseEntity
    {
        public int ItemDropId { get; set; }

        public WorkTypeEnum WorkType { set; get; }

        
        public int WareId { set; get; }  

        public int MaxNumber { set; get; }

        public int Money { set; get; }

        public int Exp { set; get; }

        public int Pot { set; get; }
        


        /// <summary>
        /// 权重，按值大小优先掉落，且总和不超过100
        /// </summary>
        public int Weight { set; get; }

        /// <summary>
        /// 奖励几率，掉落百分比
        /// </summary>
        public int Percent { set; get; }
    }
}
