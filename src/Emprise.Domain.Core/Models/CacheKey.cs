using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Models
{
    public class CacheKey
    {
        public const int ExpireMinutes = 60;


        public const string RoomList = "RoomList";
        public const string Room = "Room_{0}";
        public const string MapRoomList = "RoomList_{0}";


        public const string Script = "Script_{0}";

        public const string Ware = "Ware_{0}";


        public const string QuestList = "QuestList";
        public const string Quest = "Quest_{0}";


        public const string Npc = "Npc_{0}";
        public const string NpcList = "NpcList";


        public const string PlayerQuestList = "PlayerQuestList_{0}";
    }
}
