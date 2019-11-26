using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Models
{
    public class MailModel
    {
        public string Address { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }
    }
}
