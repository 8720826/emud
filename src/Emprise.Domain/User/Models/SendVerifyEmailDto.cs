﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Emprise.Domain.User.Models
{
    public class SendVerifyEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
