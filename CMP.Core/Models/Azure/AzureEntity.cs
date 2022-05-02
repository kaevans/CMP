using Newtonsoft.Json;
using System.Collections.Generic;

namespace CMP.Core.Models.Azure
{
    public class AzureEntity : Entity
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("tags")]
        public Dictionary<string, string> Tags { get; set; }
    }

}
