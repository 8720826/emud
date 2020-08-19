using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Queue.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Queues
{

    public class NpcStatusQueue : QueueEvent
    {
        public NpcStatusQueue(NpcStatusModel status)
        {
            Status = status;
        }


        public NpcStatusModel Status { get; set; }

    }
}
