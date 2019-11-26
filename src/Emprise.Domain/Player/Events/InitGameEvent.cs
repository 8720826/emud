using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Player.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Player.Events
{
    public class InitGameEvent : Event
    {
        public PlayerEntity Player { get; set; }


        public InitGameEvent(PlayerEntity player)
        {
            Player = player;
        }

    }
}
