using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Domain.Npc.Models
{
    public class CaseIf
    {
        [JsonProperty("condition")]
        public string Condition { get; set; }

        [JsonProperty("attrs")]
        public List<Attribute> Attrs { get; set; }
    }

    public class CaseThen
    {
        [JsonProperty("commond")]
        public string Commond { get; set; }

        [JsonProperty("attrs")]
        public List<Attribute> Attrs { get; set; }
    }

    public class CaseElse
    {
        [JsonProperty("commond")]
        public string Commond { get; set; }

        [JsonProperty("attrs")]
        public List<Attribute> Attrs { get; set; }
    }

    public class Attribute
    {
        [JsonProperty("attr")]
        public string Attr { get; set; }

        [JsonProperty("val")]
        public string Val { get; set; }
    }
}
