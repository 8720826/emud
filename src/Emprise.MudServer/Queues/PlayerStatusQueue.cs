using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Queue.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Queues
{
    public class PlayerStatusQueue : QueueEvent
    {
        public PlayerStatusQueue(PlayerStatusModel status)
        {
            Status = status;
        }


        public PlayerStatusModel Status { get; set; }

    }
}
