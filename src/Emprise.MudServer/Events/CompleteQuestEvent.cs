using Emprise.Domain.Core.Events;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Quest.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Events
{
    public class CompleteQuestEvent : Event
    {
        public PlayerEntity Player { get; set; }

        public QuestEntity Quest { get; set; }

        public CompleteQuestEvent(PlayerEntity player, QuestEntity quest)
        {
            Player = player;
            Quest = quest;
        }

    }
}
