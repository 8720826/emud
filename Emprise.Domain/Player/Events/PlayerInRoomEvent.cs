using Emprise.Domain.Core.Events;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Room.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Player.Events
{
    /// <summary>
    /// 仅仅进入房间，没有提示
    /// </summary>
    public class PlayerInRoomEvent : Event
    {
        public PlayerEntity Player { get; set; }

        public RoomEntity Room { get; set; }

        public PlayerInRoomEvent(PlayerEntity player, RoomEntity room)
        {
            Player = player;
            Room = room;
        }

    }
}
