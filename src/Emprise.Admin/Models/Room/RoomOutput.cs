using Emprise.Domain.Room.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.Room
{
    public class RoomOutput: RoomEntity
    {
        /// <summary>
        /// 东
        /// </summary>
        public RoomEntity EastRoom {  get; set; }

        /// <summary>
        /// 西
        /// </summary>
        public RoomEntity WestRoom {  get; set; }

        /// <summary>
        /// 南
        /// </summary>
        public RoomEntity SouthRoom {  get; set; }

        /// <summary>
        /// 北
        /// </summary>
        public RoomEntity NorthRoom {  get; set; }
    }
}
