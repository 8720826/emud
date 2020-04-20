using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Quest.Models
{
    public class TaskTrigger
    {
        [JsonProperty("condition")]
        public string Condition { get; set; }

        [JsonProperty("attrs")]
        public List<TaskAttribute> Attrs { get; set; }
    }

    public class TaskTarget
    {
        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("attrs")]
        public List<TaskAttribute> Attrs { get; set; }
    }

    public class TaskReward
    {
        [JsonProperty("reward")]
        public string Reward { get; set; }

        [JsonProperty("attrs")]
        public List<TaskAttribute> Attrs { get; set; }
    }

    public class TaskConsume
    {
        [JsonProperty("consume")]
        public string Consume { get; set; }

        [JsonProperty("attrs")]
        public List<TaskAttribute> Attrs { get; set; }
    }
    


    public class TaskAttribute
    {
        [JsonProperty("attr")]
        public string Attr { get; set; }

        [JsonProperty("val")]
        public string Val { get; set; }
    }
}
