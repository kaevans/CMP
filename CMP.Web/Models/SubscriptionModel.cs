using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMP.Web.Models
{
    public class SubscriptionModel
    {
        public string SubscriptionId { get; set; }
        public string ResourceGroupId { get; set; }

        public SelectList Subscriptions { get; set; }   
        public SelectList ResourceGroups { get; set; }

    }
}
