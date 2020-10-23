using Emprise.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.NpcLiking.Entity
{
    [Table("NpcLiking")]
    public class NpcLikingEntity : BaseEntity
    {
        public int PlayerId { set; get; }

        public int NpcId { set; get; }

        public int Liking { set; get; }

        public DateTime CreatedTime { set; get; }
    }
}
