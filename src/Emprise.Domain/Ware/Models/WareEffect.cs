using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Ware.Models
{

    public class WareEffect
    {
        [JsonProperty("effect")]
        public string Condition { get; set; }

        [JsonProperty("attrs")]
        public List<EffectAttribute> Attrs { get; set; }
    }

    public class EffectAttribute
    {
        [JsonProperty("attr")]
        public string Attr { get; set; }

        [JsonProperty("val")]
        public string Val { get; set; }
    }
}
