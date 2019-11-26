using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Room.Events
{
    public class UserInRoomEvent : Event
    {
        public RoomEntity Room { get; set; }

        public UserEntity User { get; set; }

        public bool NeedChangeMap { get; set; }

        public bool NeedWelcome { get; set; }


        public UserInRoomEvent(UserEntity user, RoomEntity room, bool needChangeMap, bool needWelcome)
        {
            User = user;
            Room = room;
            NeedChangeMap = needChangeMap;
            NeedWelcome = needWelcome;
        }

    }
}
