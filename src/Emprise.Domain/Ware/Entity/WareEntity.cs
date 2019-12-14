using Emprise.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Domain.Ware.Entity
{
    [Table("Ware")]
    public class WareEntity : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(20)]
        public string Name { set; get; }

        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(500)]
        public string Description { set; get; }
    }
}
