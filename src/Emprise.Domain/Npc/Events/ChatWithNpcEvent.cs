using Emprise.Domain.Core.Events;
using Emprise.Domain.Npc.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Npc.Events
{

    public class ChatWithNpcEvent : Event
    {

        public int PlayerId { get; set; }
        public int NpcId { get; set; }

        public ChatWithNpcEvent(int playerId, int npcId)
        {
            PlayerId = playerId;
            NpcId = npcId;
        }

    }
}
