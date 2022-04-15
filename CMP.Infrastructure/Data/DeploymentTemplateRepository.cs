using CMP.Core.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP.Infrastructure.Data
{
    public class DeploymentTemplateRepository : CosmosDbRepository<DeploymentTemplate>
    {
        public DeploymentTemplateRepository(
            IOptions<CosmosDbOptions> options,
            ILogger<DeploymentTemplateRepository> logger) : base (
                options, logger)
        {

        }
    }
}
