using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Emprise.Application.User.Dtos
{
    public class UserRegDto
    {

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Code { get; set; }
    }
}
