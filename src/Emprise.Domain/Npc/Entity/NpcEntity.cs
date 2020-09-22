using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Npc.Entity
{
    [Table("Npc")]
    public  class NpcEntity : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(20)]
        public string Name { set; get; }


        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(500)]
        public string Description { set; get; }


        /// <summary>
        /// 性别
        /// </summary>
        public GenderEnum Gender { set; get; }

        /// <summary>
        /// 类型
        /// </summary>
        public NpcTypeEnum Type { set; get; }

        /// <summary>
        /// 所在房间
        /// </summary>
        public int RoomId { set; get; }

        /// <summary>
        /// 是否可以聊天
        /// </summary>
        public bool CanChat { set; get; }

        /// <summary>
        /// 是否可以攻击
        /// </summary>
        public bool CanFight { set; get; }

        /// <summary>
        /// 是否可以杀死
        /// </summary>
        public bool CanKill { set; get; }


        /// <summary>
        /// 是否可以移动
        /// </summary>
        public bool CanMove { set; get; }


        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { set; get; }

        /// <summary>
        /// 容貌
        /// </summary>
        public int Per { set; get; }

        /// <summary>
        /// 福源
        /// </summary>
        public int Kar { set; get; }

        /// <summary>
        /// 实战经验
        /// </summary>
        public int Exp { set; get; }



        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { set; get; }


        public bool IsDead { set; get; }

        /// <summary>
        /// 气血
        /// </summary>
        public int Hp { set; get; }

        /// <summary>
        /// 最大气血
        /// </summary>
        public int MaxHp { set; get; }

        public int Mp { set; get; }

        /// <summary>
        /// 最大内力
        /// </summary>
        public int MaxMp { set; get; }

        /// <summary>
        /// 攻击速度（毫秒）
        /// </summary>
        public int Speed { set; get; }

    }
}
