using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Domain.Config.Models
{
    public class ConfigModel
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public Type Type { get; set; }
    }
}
