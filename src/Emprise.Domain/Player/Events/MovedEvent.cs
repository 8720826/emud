using Emprise.Domain.Core.Events;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Room.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Player.Events
{
    public class MovedEvent : Event
    {
        public PlayerEntity Player { get; set; }

        public RoomEntity RoomIn { get; set; }

        public RoomEntity RoomOut { get; set; }

        public MovedEvent(PlayerEntity player, RoomEntity roomIn, RoomEntity roomOut)
        {
            Player = player;
            RoomIn = roomIn;
            RoomOut = roomOut;
        }

    }
}
