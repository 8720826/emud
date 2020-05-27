using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Player.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Events
{
    public class CreatedEvent : Event
    {
        public PlayerEntity Player { get; set; }


        public CreatedEvent(PlayerEntity player)
        {
            Player = player;
        }

    }
}
