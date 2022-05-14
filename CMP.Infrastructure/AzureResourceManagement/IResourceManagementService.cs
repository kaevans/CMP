using CMP.Core.Models.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP.Infrastructure.AzureResourceManagement
{
    public interface IResourceManagementService
    {
        public Task<IEnumerable<Subscription>> GetSubscriptionsAsync();
        public Task<IEnumerable<ResourceGroup>> GetResourceGroupsAsync(string subscriptionId);
    }
}
