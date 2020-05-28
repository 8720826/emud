using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.Email
{
    public class EmailInput
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(200, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string Title { set; get; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(1000, ErrorMessage = "{0}最长不能超过{1}个字符")]
        public string Content { set; get; }


        /// <summary>
        /// 类型
        /// </summary>
        [Display(Name = "类型")]
        public EmailTypeEnum Type { set; get; }

        /// <summary>
        /// 相关ID
        /// </summary>
        [Display(Name = "相关ID")]
        public int TypeId { set; get; }

        /// <summary>
        /// 发送时间
        /// </summary>
        [Display(Name = "发送时间")]
        public DateTime SendDate { set; get; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [Display(Name = "过期时间")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { set; get; }
    }
}
