using Emprise.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Npc.Entity
{
    [Table("NpcItemDrop")]
    public class NpcItemDropEntity : BaseEntity
    {
        public int NpcId { get; set; }


        public int ItemDropId { get; set; }

    }
}
