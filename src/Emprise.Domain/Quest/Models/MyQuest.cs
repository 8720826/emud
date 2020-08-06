using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Quest.Models
{
    public  class MyQuest
    {
        public List<QuestModel> Quests { get; set; }

        public QuestTypeEnum Type { set; get; }
    }
}
