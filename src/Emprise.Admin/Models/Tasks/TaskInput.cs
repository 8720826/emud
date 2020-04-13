using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.Tasks
{
    public class TaskInput
    {

        public string Name { set; get; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public string Type { set; get; }

        /// <summary>
        /// 触发条件 json格式，{TaskTriggerEnum,TriggerValue}
        /// </summary>
        public string TriggerCondition { set; get; }


        /// <summary>
        /// 任务周期
        /// </summary>
        public TaskPeriodEnum Period { set; get; }

        /// <summary>
        /// 限时（分钟）
        /// </summary>
        public int TimeLimit { set; get; }

        /// <summary>
        /// 领取方式
        /// </summary>
        public TaskTakeTypeEnum TakeType { set; get; }

        /// <summary>
        /// 触发方式
        /// </summary>
        public TaskTriggerTypeEnum TriggerType { set; get; }

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
        /// 提交任务时候的提示文字
        /// </summary>
        public string BeforeCompleteWords { set; get; }

        /// <summary>
        /// 完成任务后的提示文本
        /// </summary>
        public string CompletedWords { set; get; }




        /// <summary>
        /// 交付方式
        /// </summary>
        public TaskDeliverTypeEnum DeliverType { set; get; }

        /// <summary>
        /// 任务目标 json格式，{TaskTargetEnum,TargetName,TargetNumber}
        /// </summary>
        public string Target { set; get; }


        public string Command { set; get; }

        public string Consume { set; get; }
        

        public string Reward { set; get; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { set; get; }
    }
}
