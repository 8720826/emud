using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Room.Events
{
    public class UserOutRoomEvent : Event
    {
        public RoomEntity Room { get; set; }

        public UserEntity User { get; set; }

        public UserOutRoomEvent(UserEntity user, RoomEntity room)
        {
            User = user;
            Room = room;
        }

    }
}
