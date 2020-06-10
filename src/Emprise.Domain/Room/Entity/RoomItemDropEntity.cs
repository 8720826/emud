using Emprise.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Room.Entity
{
    /// <summary>
    /// 房间掉落
    /// </summary>
    [Table("RoomItemDrop")]
    public class RoomItemDropEntity : BaseEntity
    {
        public int RoomId { get; set; }


        public int ItemDropId { get; set; }

    }
}
