using Emprise.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Queues
{
    public class ReceiveEmailQueue : QueueEvent
    {
        public ReceiveEmailQueue(int playerId)
        {
            PlayerId = playerId;
        }


        public int PlayerId { get; set; }

    }
}
