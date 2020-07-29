using Emprise.Domain.Core.Events;
using Emprise.Domain.Player.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Events
{
    public class PlayerAttributeChangedEvent : Event
    {
        public PlayerEntity Player { get; set; }

        public PlayerAttributeChangedEvent(PlayerEntity player)
        {
            Player = player;
        }

    }
}
