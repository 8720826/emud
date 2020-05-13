using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Emprise.Application.User.Models
{
    public  class UserModel
    {
        public int Id { set; get; }



        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(32)]
        public string Password { set; get; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [StringLength(50)]
        public string Email { set; get; }

        /// <summary>
        /// 邮箱是否验证
        /// </summary>
        public bool HasVerifiedEmail { set; get; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegDate { set; get; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LastDate { set; get; }


        /// <summary>
        /// 注册IP地址
        /// </summary>
        [StringLength(20)]
        public string RegIp { set; get; }

        /// <summary>
        /// 最后登录IP地址
        /// </summary>
        [StringLength(20)]
        public string LastIp { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        public PlayerStatusEnum Status { set; get; }
    }
}
