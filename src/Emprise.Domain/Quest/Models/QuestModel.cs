using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Quest.Models
{
    public class QuestModel
    {
        public  int Id { get; set; }

        public string Name { set; get; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public string Type { set; get; }


        /// <summary>
        /// 领取条件 json格式，{TaskTriggerEnum,TriggerValue}
        /// </summary>
        public string TakeCondition { set; get; }


        /// <summary>
        /// 任务周期
        /// </summary>
        public string Period { set; get; }

        /// <summary>
        /// 限时（分钟）
        /// </summary>
        public int TimeLimit { set; get; }


        /// <summary>
        /// 地图说明
        /// </summary>
        public string Description { set; get; }


        /// <summary>
        /// 任务消耗
        /// </summary>
        public string Consume { set; get; }


        /// <summary>
        /// 任务目标 json格式，{QuestTargetEnum,TargetName,TargetNumber}
        /// </summary>
        public string Target { set; get; }

        /// <summary>
        /// 任务奖励
        /// </summary>
        public string Reward { set; get; }

        public string RewardDescription { set; get; }

        public int SortId { set; get; }

    }
}
