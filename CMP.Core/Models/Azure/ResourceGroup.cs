using Newtonsoft.Json;

namespace CMP.Core.Models.Azure
{
    public class ResourceGroupJson
    {
        [JsonProperty("value")]
        public List<ResourceGroup> ResourceGroups { get; set; }
    }

    public class ResourceGroup : AzureEntity
    {
        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("managedBy")]
        public string ManagedBy { get; set; }
        
    }
}
