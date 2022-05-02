using Newtonsoft.Json;

namespace CMP.Core.Models.Azure
{
    public class SubscriptionJson
    {
        [JsonProperty("value")]
        public List<Subscription> Subscriptions { get; set; }
    }

    public class Subscription : AzureEntity
    {
        [JsonProperty(PropertyName = "subscriptionId")]
        public string SubscriptionId { get; set; }

        [JsonProperty(PropertyName = "tenantId")]
        public string TenantId { get; set; }

        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }
        
    }
}
