using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Models
{
    public class ResultModel
    {
        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }
    }
}
