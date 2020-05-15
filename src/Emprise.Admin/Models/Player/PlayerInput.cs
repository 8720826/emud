using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.Player
{
    public class PlayerInput
    {


        /// <summary>
        /// 角色名
        /// </summary>
        [Display(Name = "名称")]
        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(20, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string Name { set; get; }


        /// <summary>
        /// 等级
        /// </summary>
        [Display(Name = "等级")]
        public int Level { set; get; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public GenderEnum Gender { set; get; }

        /// <summary>
        /// 当前房间
        /// </summary>
        [Display(Name = "当前房间Id")]
        public int RoomId { set; get; }

        /// <summary>
        /// 当前门派
        /// </summary>
        [Display(Name = "门派Id")]
        public int FactionId { set; get; }

        /// <summary>
        /// 称号
        /// </summary>】
        [Display(Name = "称号")]
        public string Title { set; get; }

        /// <summary>
        /// 先天臂力（Strength）
        /// </summary>
        [Display(Name = "先天臂力")]
        public int Str { set; get; }

        /// <summary>
        /// 后天臂力
        /// </summary>
        [Display(Name = "后天臂力")]
        public int StrAdd { set; get; }

        /// <summary>
        /// 根骨（Constitution）
        /// </summary>
        [Display(Name = "先天根骨")]
        public int Con { set; get; }

        /// <summary>
        /// 后天根骨
        /// </summary>
        [Display(Name = "后天根骨")]
        public int ConAdd { set; get; }

        /// <summary>
        /// 悟性（Intelligence）
        /// </summary>
        [Display(Name = "先天悟性")]
        public int Int { set; get; }

        /// <summary>
        /// 后天悟性
        /// </summary>
        [Display(Name = "后天悟性")]
        public int IntAdd { set; get; }

        /// <summary>
        /// 身法（Dexterity）
        /// </summary>
        [Display(Name = "先天身法")]
        public int Dex { set; get; }

        /// <summary>
        /// 后天身法
        /// </summary>
        [Display(Name = "后天身法")]
        public int DexAdd { set; get; }

        /// <summary>
        /// 胆识
        /// </summary>
        [Display(Name = "胆识")]
        public int Cor { set; get; }

        /// <summary>
        /// 定力
        /// </summary>
        [Display(Name = "定力")]
        public int Cps { set; get; }

        /// <summary>
        /// 容貌
        /// </summary>
        [Display(Name = "容貌")]
        public int Per { set; get; }

        /// <summary>
        /// 福缘
        /// </summary>
        [Display(Name = "福缘")]
        public int Kar { set; get; }

        /// <summary>
        /// 气血
        /// </summary>
        [Display(Name = "气血")]
        public int Hp { set; get; }

        /// <summary>
        /// 最大气血
        /// </summary>
        [Display(Name = "最大气血")]
        public int MaxHp { set; get; }

        /// <summary>
        /// 精力
        /// </summary>
        [Display(Name = "精力")]
        public int Nrg { set; get; }


        /// <summary>
        /// 内力
        /// </summary>
        [Display(Name = "内力")]
        public int Mp { set; get; }

        /// <summary>
        /// 最大内力
        /// </summary>
        [Display(Name = "最大内力")]
        public int MaxMp { set; get; }

        /// <summary>
        /// 内力上限
        /// </summary>
        [Display(Name = "内力上限")]
        public int LimitMp { set; get; }

        /// <summary>
        /// 经验
        /// </summary>
        [Display(Name = "经验")]
        public int Exp { set; get; }

        /// <summary>
        /// 潜能
        /// </summary>
        [Display(Name = "潜能")]
        public int Pot { set; get; }

        /// <summary>
        /// 攻击
        /// </summary>
        [Display(Name = "攻击")]
        public int Atk { set; get; }

        /// <summary>
        /// 防御
        /// </summary>
        [Display(Name = "防御")]
        public int Def { set; get; }

        /// <summary>
        /// 命中
        /// </summary>
        [Display(Name = "命中")]
        public int Hit { set; get; }

        /// <summary>
        /// 闪避
        /// </summary>
        [Display(Name = "闪避")]
        public int Flee { set; get; }

        /// <summary>
        /// 招架
        /// </summary>
        [Display(Name = "招架")]
        public int Parry { set; get; }

        /// <summary>
        /// 铜板
        /// 1000铜板=1两银子
        /// </summary>
        [Display(Name = "金钱")]
        public long Money { set; get; }

        /// <summary>
        /// 年龄（月数，计算年龄要除以12）
        /// </summary>
        [Display(Name = "年龄(月)")]
        public int Age { set; get; }


        /// <summary>
        /// 可分配点数
        /// </summary>
        [Display(Name = "可分配点数")]
        public int Point { set; get; }
    }
}
