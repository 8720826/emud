using Emprise.Domain.Core.Events;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Room.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Events
{
    public class NpcMovedEvent : Event
    {
        public NpcEntity Npc { get; set; }

        public RoomEntity RoomIn { get; set; }

        public RoomEntity RoomOut { get; set; }

        public NpcMovedEvent(NpcEntity npc, RoomEntity roomIn, RoomEntity roomOut)
        {
            Npc = npc;
            RoomIn = roomIn;
            RoomOut = roomOut;
        }

    }
}
