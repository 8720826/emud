using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Player.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Events
{
    public class JoinedGameEvent : Event
    {
        public PlayerEntity Player { get; set; }


        public JoinedGameEvent(PlayerEntity player)
        {
            Player = player;
        }

    }
}
