using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Emprise.Domain.User.Models
{
    public class ModifyPasswordDto
    {

        /// <summary>
        /// 原密码
        /// </summary>
        [Required(ErrorMessage = "请输入原密码")]
        public string Password { get; set; }


        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage = "请输入新密码")]
        public string NewPassword { get; set; }

    }
}
