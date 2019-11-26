using Emprise.Domain.Room.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Application.Room.Models
{
    public class CurrentRoomInfo
    {
        public RoomModel CurrentRoom { get; set; }

        /// <summary>
        /// 东
        /// </summary>
        public RoomModel EastRoom { set; get; }

        /// <summary>
        /// 西
        /// </summary>
        public RoomModel WestRoom { set; get; }

        /// <summary>
        /// 南
        /// </summary>
        public RoomModel SouthRoom { set; get; }

        /// <summary>
        /// 北
        /// </summary>
        public RoomModel NorthRoom { set; get; }
    }


}
