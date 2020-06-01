using Emprise.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Events.EmailEvents
{
    public class DeletedEmailEvent : Event
    {
        public int PlayerId { get; set; }

        public int PlayerEmailId { get; set; }
        public DeletedEmailEvent(int playerId, int playerEmailId)
        {
            PlayerId = playerId;
            PlayerEmailId = playerEmailId;
        }


    }
}
