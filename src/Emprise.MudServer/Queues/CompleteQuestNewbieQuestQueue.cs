using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Queues
{

     public class CompleteQuestNewbieQuestQueue : QueueEvent
    {
        public CompleteQuestNewbieQuestQueue(int playerId, NewbieQuestEnum quest)
        {
            PlayerId = playerId;
            Quest = quest;
        }


        public int PlayerId { get; set; }

        public NewbieQuestEnum Quest { get; set; }
    }
}
