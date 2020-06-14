using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Application.Admin.Dtos
{
    public class LoginInput
    {
        [Display(Name = "账号")]
        [Required(ErrorMessage = "请输入{0}")]
        public string Name { get; set; }

        [Display(Name = "密码")]
        [Required(ErrorMessage = "请输入{0}")]
        public string Password { get; set; }
    }
}
