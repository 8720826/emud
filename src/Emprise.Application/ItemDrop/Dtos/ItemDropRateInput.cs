using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Emprise.Application.ItemDrop.Dtos
{
    public class ItemDropRateInput
    {
      




        [Display(Name = "掉落类型")]
        [Required(ErrorMessage = "请选择{0}")]
        public ItemDropTypeEnum DropType { set; get; }

        [Display(Name = "掉落顺序")]
        public int Order { set; get; }

        [Display(Name = "物品Id")]
        public int? WareId { set; get; }


        [Display(Name = "最大数量")]
        public int? MaxNumber { set; get; }

        [Display(Name = "最小数量")]
        public int? MinNumber { set; get; }



        /// <summary>
        /// 权重，按值大小优先掉落，且总和不超过100
        /// </summary>
        [Display(Name = "权重")]
        public int Weight { set; get; }

        /// <summary>
        /// 奖励几率，掉落百分比
        /// </summary>
        [Display(Name = "掉落几率")]
        public int Percent { set; get; }
    }
}
