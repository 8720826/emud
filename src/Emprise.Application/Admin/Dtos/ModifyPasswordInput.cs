using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Application.Admin.Dtos
{
    public class ModifyPasswordInput
    {
        [Display(Name = "原密码")]
        [Required(ErrorMessage = "请输入原密码")]
        public string Password { get; set; }


        [Display(Name = "新密码")]
        [Required(ErrorMessage = "请输入新密码")]
        public string NewPassword { get; set; }

        [Display(Name = "重复新密码")]
        [Required(ErrorMessage = "请再次输入新密码")]
        [Compare("NewPassword", ErrorMessage = "两次新密码必须相同")]
        public string ConfirmPassword { get; set; }
        
    }
}
