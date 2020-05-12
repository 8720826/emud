using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Models.Chat
{
    public class PlayerOnlineModel
    {
        /// <summary>
        /// playerId
        /// </summary>
        public int PlayerId { set; get; }

        public string PlayerName { set; get; }

        public GenderEnum Gender { set; get; }

        public int Level { set; get; }

        public DateTime LastDate { set; get; }

        public int RoomId { set; get; }


        public string Title { set; get; }

        /// <summary>
        /// 在线还是离线
        /// </summary>
        public bool IsOnline { set; get; }
    }
}
