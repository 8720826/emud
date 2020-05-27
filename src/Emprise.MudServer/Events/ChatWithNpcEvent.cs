using Emprise.Domain.Core.Events;
using Emprise.Domain.Npc.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Events
{

    public class ChatWithNpcEvent : Event
    {

        public int PlayerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int NpcId { get; set; }

        public ChatWithNpcEvent(int playerId, int npcId)
        {
            PlayerId = playerId;
            NpcId = npcId;
        }

    }
}
