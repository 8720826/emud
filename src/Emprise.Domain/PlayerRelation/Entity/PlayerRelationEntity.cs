using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.PlayerRelation.Entity
{
    [Table("PlayerRelation")]
    public class PlayerRelationEntity : BaseEntity
    {
        public int PlayerId { set; get; }

        public PlayerRelationTypeEnum Type { set; get; }

        public int RelationId { set; get; }

        public DateTime CreatedTime { set; get; }


    }
}
