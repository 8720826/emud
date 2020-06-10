using Emprise.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.ItemDrop.Entity
{
    [Table("ItemDrop")]
    public class ItemDropEntity : BaseEntity
    {
        public string Name { set; get; }

        /// <summary>
        /// 脚本描述
        /// </summary>
        [StringLength(500)]
        public string Description { set; get; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { set; get; }
    }
}
