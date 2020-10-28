using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Skill.Models
{
    public class SkillModel
    {
        public int Id { set; get; }

        public int ObjectSkillId { set; get; }

        public int ObjectType { set; get; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// 是否基础技能
        /// </summary>
        public bool IsBase { set; get; }

        /// <summary>
        /// 大类
        /// </summary>
        public SkillCategoryEnum Category { set; get; }

        /// <summary>
        /// 类型
        /// </summary>
        public SkillTypeEnum Type { set; get; }

        /// <summary>
        /// 冷却时间
        /// </summary>
        public int CoolDownTime { set; get; }

        /// <summary>
        /// 使用时消耗内力
        /// </summary>
        public int UseForce { set; get; }


        public int Level { get; set; }

        public int Exp { get; set; }

    }
}
