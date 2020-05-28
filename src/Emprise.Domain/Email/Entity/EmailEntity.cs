using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Email.Entity
{
    [Table("Email")]
    public class EmailEntity : BaseEntity
    {   
        
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { set; get; }


        /// <summary>
        /// 类型
        /// </summary>
        public EmailTypeEnum Type { set; get; }

        /// <summary>
        /// 相关ID
        /// </summary>
        public int TypeId { set; get; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendDate { set; get; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpiryDate { set; get; }
    }
}
