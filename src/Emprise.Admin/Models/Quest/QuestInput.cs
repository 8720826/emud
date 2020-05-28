using Emprise.Domain.Core.Attributes;
using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.Quest
{
    public class QuestInput
    {
        [Display(Name = "任务名称")]
        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(50, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string Name { set; get; }

        /// <summary>
        /// 任务类型
        /// </summary>
        [Display(Name = "任务类型")]
        [EnumValidation(typeof(QuestTypeEnum), ErrorMessage = "请选择{0}")]
        [Required]
        public QuestTypeEnum Type { set; get; }

        /// <summary>
        /// 触发条件 json格式，{TaskTriggerEnum,TriggerValue}
        /// </summary>
        [Display(Name = "触发条件")]
        [StringLength(4000, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string TriggerCondition { set; get; }

        /// <summary>
        /// 领取条件 json格式，{TaskTriggerEnum,TriggerValue}
        /// </summary>
        [Display(Name = "领取条件")]
        [StringLength(4000, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string TakeCondition { set; get; }


        /// <summary>
        /// 任务周期
        /// </summary>
        [Display(Name = "任务周期")]
        [EnumValidation(typeof(QuestPeriodEnum), ErrorMessage = "请选择{0}")]
        public QuestPeriodEnum Period { set; get; }

        /// <summary>
        /// 限时（分钟）
        /// </summary>
        [Display(Name = "限时（分钟）")]
        public int TimeLimit { set; get; }

        
        [Display(Name = "排序")]
        public int SortId { set; get; }

        [Display(Name = "描述")]
        [StringLength(4000, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string Description { set; get; }


        /// <summary>
        /// 任务目标 json格式，{QuestTargetEnum,TargetName,TargetNumber}
        /// </summary>
        [Display(Name = "任务目标")]
        [StringLength(4000, ErrorMessage = "{0}最长不能超过{1}个字符")]
        [Required(ErrorMessage = "请填写内容")]
        public string Target { set; get; }


        [Display(Name = "任务消耗")]
        [StringLength(4000, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string Consume { set; get; }

        [Display(Name = "任务奖励")]
        [StringLength(4000, ErrorMessage = "{0}最长不能超过{1}个字符")]
        [Required(ErrorMessage = "请设置任务奖励")]
        public string Reward { set; get; }


    }
}
