using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP.Infrastructure.Cloud.Azure.ResourceManagement
{
    public class ResourceManagementOptions
    {
        public static string SectionName = "Azure";

        public string APIVersion { get; set; }
        public string APIEndpoint { get; set; }
        public string TenantId { get; set; }
    }
}
