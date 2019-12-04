using Emprise.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Room.Entity
{
    [Table("Room")]
    public class RoomEntity : BaseEntity
    {




        /// <summary>
        /// 房间名
        /// </summary>
        [StringLength(32)]
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
        [StringLength(500)]
        public string Description { set; get; }

        /// <summary>
        /// 是否可以战斗
        /// </summary>
        [Column(TypeName = "bit")]
        public bool CanFight { set; get; }

        /// <summary>
        /// 是否可以挖矿
        /// </summary>
        [Column(TypeName = "bit")]
        public bool CanDig { set; get; }

        /// <summary>
        /// 是否可以伐木
        /// </summary>
        [Column(TypeName = "bit")]
        public bool CanCut { set; get; }

        /// <summary>
        /// 是否可以钓鱼
        /// </summary>
        [Column(TypeName = "bit")]
        public bool CanFish { set; get; }

        /// <summary>
        /// 是否可以采药
        /// </summary>
        [Column(TypeName = "bit")]
        public bool CanCollect { set; get; }

        /// <summary>
        /// 是否可以打猎
        /// </summary>
        [Column(TypeName = "bit")]
        public bool CanHunt { set; get; }
    }
}
