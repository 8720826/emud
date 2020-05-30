using Emprise.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Events.EmailEvents
{
    public class ReceivedEmailEvent : Event
    {
        public int PlayerId { get; set; }


        public ReceivedEmailEvent(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
