using CMP.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CMP.Infrastructure.CosmosDb
{
    public class DeploymentTemplateRepository : CosmosDbRepository<DeploymentTemplate>
    {
        public DeploymentTemplateRepository(
            IOptions<CosmosDbOptions> options,
            ILogger<DeploymentTemplateRepository> logger) : base(
        options, logger)
        {

        }
    }
}
