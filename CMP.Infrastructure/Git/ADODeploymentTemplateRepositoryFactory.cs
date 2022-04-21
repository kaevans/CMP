using CMP.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP.Infrastructure.Git
{
    public class ADODeploymentTemplateRepositoryFactory : IGitRepoItemRepositoryFactory<DeploymentTemplate>
    {
        private readonly ILogger<ADODeploymentTemplateRepositoryFactory> _logger;
        private readonly GitRepoOptions _options;
        public ADODeploymentTemplateRepositoryFactory(
            ILogger<ADODeploymentTemplateRepositoryFactory> logger,
            IOptions<GitRepoOptions> options)
        { 
            _logger = logger;
            _options = options.Value;
        }

        public IGitRepoItemRepository<DeploymentTemplate> GetRepoItemRepository(GitRepo repo, ILogger logger)
        {
            return new ADODeploymentTemplateRepository(repo, _options, _logger);
        }
    }
}
