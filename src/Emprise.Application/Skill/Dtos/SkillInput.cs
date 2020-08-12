using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Emprise.Application.Skill.Dtos
{
    public class SkillInput
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
    }
}
