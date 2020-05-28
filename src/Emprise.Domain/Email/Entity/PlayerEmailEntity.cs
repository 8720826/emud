using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Email.Entity
{
    [Table("PlayerEmail")]
    public class PlayerEmailEntity : BaseEntity
    {
        /// <summary>
        /// 玩家id
        /// </summary>
        public int PlayerId { set; get; }

        /// <summary>
        /// 通知id
        /// </summary>
        public int MessageId { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        public EmailStatusEnum Status { set; get; }

        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime CreateDate { set; get; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpiryDate { set; get; }
    }
}
