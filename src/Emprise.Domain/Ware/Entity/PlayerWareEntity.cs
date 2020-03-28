using Emprise.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Ware.Entity
{
    [Table("PlayerWare")]
    public class PlayerWareEntity : BaseEntity
    {
        public int PlayerId { get; set; }

        public int WareId { get; set; }

        public int WareName { get; set; }

        public int Number { get; set; }
    }
}
