using Emprise.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Quest.Entity
{
    [Table("PlayerQuest")]
    public class PlayerQuestEntity : BaseEntity
    {
        public int PlayerId { set; get; }

        public int QuestId { set; get; }


        /// <summary>
        /// 领取时间
        /// </summary>
        public DateTime CreateDate { set; get; }

        /// <summary>
        /// 目标
        /// </summary>
        public string Target { set; get; }



        /// <summary>
        /// 已领取次数
        /// </summary>
        public int Times { set; get; }

        /// <summary>
        /// 今天已领取次数
        /// </summary>
        public int DayTimes { set; get; }



        /// <summary>
        /// 是否领取任务
        /// </summary>
        public bool HasTake { set; get; }


        /// <summary>
        /// 更新实际
        /// </summary>
        public DateTime UpdateDate { set; get; }

        /// <summary>
        /// 完成实际
        /// </summary>
        public DateTime CompleteDate { set; get; }

        /// <summary>
        /// 上次领取时间
        /// </summary>
        public DateTime TakeDate { set; get; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplete { set; get; }
    }
}
