using Newtonsoft.Json;

namespace CMP.Core.Models
{
    public class DeploymentTemplate : Entity
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "imageurl")]
        public string ImageUrl { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        public void ParseContents(string contents)
        {
            Description = contents;
        }
    }
}
