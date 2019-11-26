using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Emprise.Domain.Core.Entity
{
    /// <summary>
    /// 实体类基类
    /// </summary>
    public class BaseEntity : IEntity
    {
        /// <summary>
        /// 自增Id
        /// </summary>
        [Key]
        public virtual int Id { get; set; }

    }
}
