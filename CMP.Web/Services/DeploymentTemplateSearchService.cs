using CMP.Infrastructure.Search;
using CMP.Web.Models;
using Microsoft.Extensions.Options;

namespace CMP.Web.Services
{
    public class DeploymentTemplateSearchService : AzureSearchService<DeploymentTemplateSearchResult>
    {
        public DeploymentTemplateSearchService(
            IOptions<AzureSearchOptions> azureSearchOptions,
            ILogger<DeploymentTemplateSearchService> logger) : base(azureSearchOptions, logger) { }
    }
}
