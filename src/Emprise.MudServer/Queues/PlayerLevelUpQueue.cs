using Emprise.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Queues
{
    public class PlayerLevelUpQueue : QueueEvent
    {
        public PlayerLevelUpQueue(int playerId)
        {
            PlayerId = playerId;
        }


        public int PlayerId { get; set; }

    }
}
