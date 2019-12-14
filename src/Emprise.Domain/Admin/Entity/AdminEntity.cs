using Emprise.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Admin.Entity
{
    [Table("Admin")]
    public class AdminEntity : BaseEntity
    {

        /// <summary>
        /// 账号
        /// </summary>
        [StringLength(32)]
        public string Name {  get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(32)]
        public string Password {  get; set; }
    }
}
