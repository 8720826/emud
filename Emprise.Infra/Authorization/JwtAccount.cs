using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Infra.Authorization
{
    public class JwtAccount
    {
        /// <summary>
        /// 登录ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 聊天id
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// 角色id
        /// </summary>
        public int PlayerId { get; set; }


        /// <summary>
        /// 角色名
        /// </summary>
        public string PlayerName { get; set; }
    }
}
