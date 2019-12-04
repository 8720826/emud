using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Room.Models
{
    public class RoomModel
    {
        public int Id { set; get; }

        /// <summary>
        /// 房间名
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 东
        /// </summary>
        public int East { get; set; }

        public string EastName { get; set; }

        /// <summary>
        /// 西
        /// </summary>
        public int West { get; set; }

        public string WestName { get; set; }

        /// <summary>
        /// 南
        /// </summary>
        public int South { get; set; }

        public string SouthName { get; set; }

        /// <summary>
        /// 北
        /// </summary>
        public int North { get; set; }

        public string NorthName { get; set; }

        /// <summary>
        /// 房间说明
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// 是否可以战斗
        /// </summary>
        public bool CanFight { set; get; }

        /// <summary>
        /// 是否可以挖矿
        /// </summary>
        public bool CanDig { set; get; }

        /// <summary>
        /// 是否可以伐木
        /// </summary>
        public bool CanCut { set; get; }

        /// <summary>
        /// 是否可以钓鱼
        /// </summary>
        public bool CanFish { set; get; }

        /// <summary>
        /// 是否可以采药
        /// </summary>
        public bool CanCollect { set; get; }

        /// <summary>
        /// 是否可以打猎
        /// </summary>
        public bool CanHunt { set; get; }
    }
}
