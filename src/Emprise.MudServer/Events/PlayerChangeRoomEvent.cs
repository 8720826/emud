using Emprise.Domain.Core.Events;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Room.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Events
{
    public class PlayerChangeRoomEvent : Event
    {
        public PlayerEntity Player { get; set; }

        public RoomEntity RoomIn { get; set; }

        public RoomEntity RoomOut { get; set; }

        public PlayerChangeRoomEvent(PlayerEntity player, RoomEntity roomIn, RoomEntity roomOut)
        {
            Player = player;
            RoomIn = roomIn;
            RoomOut = roomOut;
        }

    }
}
