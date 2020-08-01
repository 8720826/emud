using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Ware.Models
{
    public class Weapon
    {
        public string Part { get; set; }

        public WareModel Ware { get; set; }

    }
}
