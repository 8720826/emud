using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Player.Entity
{
    [Table("Player")]
    public class PlayerEntity : BaseEntity
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { set; get; }

        /// <summary>
        /// 角色名
        /// </summary>
        [StringLength(20)]
        public string Name { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LastDate { set; get; }



        /// <summary>
        /// 状态，主动进行且会持续比较长时间的状态
        /// 如：打坐、疗伤、工作等
        /// 当条件不符合的情况下会自动取消，如疗伤完毕、体力耗尽、角色死亡等
        /// 玩家也可以主动取消
        /// </summary>
        public PlayerStatusEnum Status { set; get; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { set; get; }

        /// <summary>
        /// 性别
        /// </summary>
        public GenderEnum Gender { set; get; }

        /// <summary>
        /// 当前场景
        /// </summary>
        public int RoomId { set; get; }

        /// <summary>
        /// 当前门派
        /// </summary>
        public int FactionId { set; get; }

        /// <summary>
        /// 称号
        /// </summary>
        public string Title { set; get; }

        /// <summary>
        /// 先天臂力（Strength）
        /// </summary>
        public int Str { set; get; }

        /// <summary>
        /// 后天臂力
        /// </summary>
        public int StrAdd { set; get; }

        /// <summary>
        /// 根骨（Constitution）
        /// </summary>
        public int Con { set; get; }

        /// <summary>
        /// 后天根骨
        /// </summary>
        public int ConAdd { set; get; }

        /// <summary>
        /// 悟性（Intelligence）
        /// </summary>
        public int Int { set; get; }

        /// <summary>
        /// 后天悟性
        /// </summary>
        public int IntAdd { set; get; }

        /// <summary>
        /// 身法（Dexterity）
        /// </summary>
        public int Dex { set; get; }

        /// <summary>
        /// 后天身法
        /// </summary>
        public int DexAdd { set; get; }

        /// <summary>
        /// 胆识
        /// </summary>
        public int Cor { set; get; }

        /// <summary>
        /// 定力
        /// </summary>
        public int Cps { set; get; }

        /// <summary>
        /// 容貌
        /// </summary>
        public int Per { set; get; }

        /// <summary>
        /// 福缘
        /// </summary>
        public int Kar { set; get; }

        /// <summary>
        /// 气血
        /// </summary>
        public int Hp { set; get; }

        /// <summary>
        /// 最大气血
        /// </summary>
        public int MaxHp { set; get; }

        /// <summary>
        /// 精力
        /// </summary>
        public int Nrg { set; get; }
        

        /// <summary>
        /// 内力
        /// </summary>
        public int Mp { set; get; }

        /// <summary>
        /// 最大内力
        /// </summary>
        public int MaxMp { set; get; }

        /// <summary>
        /// 内力上限
        /// </summary>
        public int LimitMp { set; get; }

        /// <summary>
        /// 经验
        /// </summary>
        public int Exp { set; get; }

        /// <summary>
        /// 潜能
        /// </summary>
        public int Pot { set; get; }

        /// <summary>
        /// 攻击
        /// </summary>
        public int Atk { set; get; }

        /// <summary>
        /// 防御
        /// </summary>
        public int Def { set; get; }

        /// <summary>
        /// 命中
        /// </summary>
        public int Hit { set; get; }

        /// <summary>
        /// 闪避
        /// </summary>
        public int Flee { set; get; }

        /// <summary>
        /// 招架
        /// </summary>
        public int Parry { set; get; }

        /// <summary>
        /// 铜板
        /// 1000铜板=1两银子
        /// </summary>
        public long Money { set; get; }

        /// <summary>
        /// 年龄（月数，计算年龄要除以12）
        /// </summary>
        public int Age { set; get; }

        /// <summary>
        /// 权限
        /// 此处为管理权限，如建造房间
        /// </summary>
        public string Auths { set; get; }

        /// <summary>
        /// 可分配点数
        /// </summary>
        public int Point { set; get; }


    }
}


