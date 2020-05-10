using Emprise.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Map.Entity
{
    [Table("Map")]
    public class MapEntity : BaseEntity
    {

        /// <summary>
        /// 地图名
        /// </summary>
        [StringLength(32)]
        public string Name { set; get; }

        /// <summary>
        /// 地图说明
        /// </summary>
        [StringLength(500)]
        public string Description { set; get; }



    }
}
