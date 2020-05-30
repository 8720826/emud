using Emprise.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Events.EmailEvents
{

    public class EmailStatusChangedEvent : Event
    {
        public int PlayerId { get; set; }


        public EmailStatusChangedEvent(int playerId)
        {
            PlayerId = playerId;
        }

    }
}
