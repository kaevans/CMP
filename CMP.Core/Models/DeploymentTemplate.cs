using Newtonsoft.Json;

namespace CMP.Core.Models
{
    public class DeploymentTemplate : GitRepoItem
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "itemDisplayName")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }

        [JsonProperty(PropertyName = "readmeurl")]
        public string ReadmeUrl { get; set; }
    }
}
