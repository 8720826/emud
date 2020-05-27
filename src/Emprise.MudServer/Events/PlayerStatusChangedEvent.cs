using Emprise.Domain.Core.Events;
using Emprise.Domain.Player.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Events
{
    public class PlayerStatusChangedEvent : Event
    {
        public PlayerEntity Player { get; set; }

        public PlayerStatusChangedEvent(PlayerEntity player)
        {
            Player = player;
        }

    }
}
