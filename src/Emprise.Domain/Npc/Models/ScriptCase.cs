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
        public List<CaseAttribute> Attrs { get; set; }
    }

    public class CaseThen
    {
        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("attrs")]
        public List<CaseAttribute> Attrs { get; set; }
    }

    public class CaseElse
    {
        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("attrs")]
        public List<CaseAttribute> Attrs { get; set; }
    }

    public class CaseAttribute
    {
        [JsonProperty("attr")]
        public string Attr { get; set; }

        [JsonProperty("val")]
        public string Val { get; set; }
    }
}
