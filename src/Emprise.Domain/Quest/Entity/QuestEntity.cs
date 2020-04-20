using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Domain.Quest.Entity
{
    [Table("Quest")]
    public class QuestEntity : BaseEntity
    {
        public string Name { set; get; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public QuestTypeEnum Type { set; get; }

        /// <summary>
        /// 触发方式
        /// </summary>
        public QuestTriggerTypeEnum TriggerType { set; get; }


        /// <summary>
        /// 触发条件 json格式，{TaskTriggerEnum,TriggerValue}
        /// </summary>
        public string TriggerCondition { set; get; }


        /// <summary>
        /// 领取条件 json格式，{TaskTriggerEnum,TriggerValue}
        /// </summary>
        public string TakeCondition { set; get; }


        /// <summary>
        /// 任务周期
        /// </summary>
        public QuestPeriodEnum Period { set; get; }

        /// <summary>
        /// 限时（分钟）
        /// </summary>
        public int TimeLimit { set; get; }

        /// <summary>
        /// 领取方式
        /// </summary>
        public QuestTakeTypeEnum TakeType { set; get; }

        /// <summary>
        /// 创建任务前的提示文本
        /// </summary>
        public string BeforeCreate { set; get; }

        /// <summary>
        /// 创建任务后的提示文本
        /// </summary>

        public string CreatedWords { set; get; }

        /// <summary>
        /// 任务进行中的提示文本
        /// </summary>
        public string InProgressWords { set; get; }

        /// <summary>
        /// 完成任务后的提示文本
        /// </summary>
        public string CompletedWords { set; get; }


        /// <summary>
        /// 任务消耗
        /// </summary>
        public string Consume { set; get; }

        /// <summary>
        /// 交付方式
        /// </summary>
        public QuestDeliverTypeEnum DeliverType { set; get; }

        /// <summary>
        /// 任务目标 json格式，{QuestTargetEnum,TargetName,TargetNumber}
        /// </summary>
        public string Target { set; get; }

        /// <summary>
        /// 任务奖励
        /// </summary>
        public string Reward { set; get; }

        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(500)]
        public string Description { set; get; }
    }
}
