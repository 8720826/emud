using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Ware.Entity
{
    [Table("PlayerWare")]
    public class PlayerWareEntity : BaseEntity
    {
        public int PlayerId { get; set; }

        public int WareId { get; set; }

        public int WareName { get; set; }

        public int Number { get; set; }

        /// <summary>
        /// 损坏度
        /// </summary>
        public int Damage { set; get; }

        /// <summary>
        /// 状态
        /// </summary>

        public WareStatusEnum Status { set; get; }

        /// <summary>
        /// 是否绑定（绑定后，被杀不掉落。绑定后不能交易、赠送）
        /// </summary>
        public bool IsBind { set; get; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { set; get; }


        /// <summary>
        /// 是否临时(为true，则下线后消失)
        /// </summary>
        public bool IsTemp { set; get; }
    }
}
