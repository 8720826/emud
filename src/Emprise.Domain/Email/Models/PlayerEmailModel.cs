using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Email.Models
{
    public class PlayerEmailModel
    {
        public int Id { set; get; }

        /// <summary>
        /// 玩家id
        /// </summary>
        public int PlayerId { set; get; }

        /// <summary>
        /// 邮件id
        /// </summary>
        public int EmailId { set; get; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        public EmailStatusEnum Status { set; get; }

        /// <summary>
        /// 接收时间
        /// </summary>
        public string CreateDate { set; get; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpiryDate { set; get; }

        public bool IsShow { set; get; }
    }
}
