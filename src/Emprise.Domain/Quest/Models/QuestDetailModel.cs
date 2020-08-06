using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Quest.Models
{
    public class QuestDetailModel
    {
        public int Id { get; set; }

        public string Name { set; get; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public string Type { set; get; }



        /// <summary>
        /// 任务周期
        /// </summary>
        public string Period { set; get; }

        /// <summary>
        /// 限时（分钟）
        /// </summary>
        public int TimeLimit { set; get; }


        /// <summary>
        /// 说明
        /// </summary>
        public string Description { set; get; }


        public string RewardDescription { set; get; }

        public int SortId { set; get; }

    }
}
