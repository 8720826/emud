using Emprise.Domain.Core.Attributes;
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
        [ConditionField(PlayerConditionFieldEnum.状态)]
        public PlayerStatusEnum Status { set; get; }

        /// <summary>
        /// 等级
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.等级)]
        public int Level { set; get; }

        /// <summary>
        /// 性别
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.性别)]
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
        [ConditionField(PlayerConditionFieldEnum.先天臂力)]

        public int Str { set; get; }

        /// <summary>
        /// 后天臂力
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.后天臂力)]
        public int StrAdd { set; get; }

        /// <summary>
        /// 根骨（Constitution）
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.先天根骨)]
        public int Con { set; get; }

        /// <summary>
        /// 后天根骨
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.后天根骨)]
        public int ConAdd { set; get; }

        /// <summary>
        /// 悟性（Intelligence）
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.先天悟性)]
        public int Int { set; get; }

        /// <summary>
        /// 后天悟性
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.后天悟性)]
        public int IntAdd { set; get; }

        /// <summary>
        /// 身法（Dexterity）
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.先天身法)]
        public int Dex { set; get; }

        /// <summary>
        /// 后天身法
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.后天身法)]
        public int DexAdd { set; get; }

        /// <summary>
        /// 胆识
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.胆识)]
        public int Cor { set; get; }

        /// <summary>
        /// 定力
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.定力)]
        public int Cps { set; get; }

        /// <summary>
        /// 容貌
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.容貌)]
        public int Per { set; get; }

        /// <summary>
        /// 福缘
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.福缘)]
        public int Kar { set; get; }

        /// <summary>
        /// 气血
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.气血)]
        public int Hp { set; get; }

        /// <summary>
        /// 最大气血
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.最大气血)]
        public int MaxHp { set; get; }

        /// <summary>
        /// 精力
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.精力)]
        public int Nrg { set; get; }


        /// <summary>
        /// 内力
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.内力)]
        public int Mp { set; get; }

        /// <summary>
        /// 最大内力
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.最大内力)]
        public int MaxMp { set; get; }

        /// <summary>
        /// 内力上限
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.内力上限)]
        public int LimitMp { set; get; }

        /// <summary>
        /// 经验
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.经验)]
        public int Exp { set; get; }

        /// <summary>
        /// 潜能
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.潜能)]
        public int Pot { set; get; }

        /// <summary>
        /// 攻击
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.攻击)]
        public int Atk { set; get; }

        /// <summary>
        /// 防御
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.防御)]
        public int Def { set; get; }

        /// <summary>
        /// 命中
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.命中)]
        public int Hit { set; get; }

        /// <summary>
        /// 闪避
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.闪避)]
        public int Flee { set; get; }

        /// <summary>
        /// 招架
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.招架)]
        public int Parry { set; get; }

        /// <summary>
        /// 铜板
        /// 1000铜板=1两银子
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.金钱)]
        public long Money { set; get; }

        /// <summary>
        /// 年龄（月数，计算年龄要除以12）
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.年龄)]
        public int Age { set; get; }

        /// <summary>
        /// 权限
        /// 此处为管理权限，如建造房间
        /// </summary>
        public string Auths { set; get; }

        /// <summary>
        /// 可分配点数
        /// </summary>
        [ConditionField(PlayerConditionFieldEnum.可分配点数)]
        public int Point { set; get; }


    }
}


