using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Quest.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Events
{
    /// <summary>
    /// 任务触发器事件
    /// </summary>
    public class QuestTriggerEvent : Event
    {
        public PlayerEntity Player { get; set; }

        public QuestTriggerTypeEnum QuestTriggerType { get; set; }

        public QuestTriggerEvent(PlayerEntity player, QuestTriggerTypeEnum questTriggerType)
        {
            Player = player;
            QuestTriggerType = questTriggerType;
        }

    }
}
