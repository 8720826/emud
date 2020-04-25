using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Quest.Models
{
    public class QuestTrigger
    {
        [JsonProperty("condition")]
        public string Condition { get; set; }

        [JsonProperty("attrs")]
        public List<QuestAttribute> Attrs { get; set; }
    }


    public class QuestTakeCondition
    {
        [JsonProperty("condition")]
        public string Condition { get; set; }

        [JsonProperty("attrs")]
        public List<QuestAttribute> Attrs { get; set; }
    }

    public class QuestTarget
    {
        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("attrs")]
        public List<QuestAttribute> Attrs { get; set; }
    }

    public class QuestReward
    {
        [JsonProperty("reward")]
        public string Reward { get; set; }

        [JsonProperty("attrs")]
        public List<QuestAttribute> Attrs { get; set; }
    }

    public class QuestConsume
    {
        [JsonProperty("consume")]
        public string Consume { get; set; }

        [JsonProperty("attrs")]
        public List<QuestAttribute> Attrs { get; set; }
    }
    


    public class QuestAttribute
    {
        [JsonProperty("attr")]
        public string Attr { get; set; }

        [JsonProperty("val")]
        public string Val { get; set; }
    }
}
