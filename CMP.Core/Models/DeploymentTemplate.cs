using Newtonsoft.Json;

namespace CMP.Core.Models
{
    public class DeploymentTemplate : GitRepoItem
    {
        [JsonProperty(PropertyName = "$schema")]
        public string Schema { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "itemDisplayName")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }

        [JsonProperty(PropertyName = "gitUsername")]
        public string GitUserName { get; set; }

        [JsonProperty(PropertyName = "dateUpdated")]
        public string DateUpdated { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "readmeurl")]
        public string ReadmeUrl { get; set; }        



    }
}
