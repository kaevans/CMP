using CMP.Core.Interfaces;
using CMP.Functions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading.Tasks;
using CMPGitRepository = CMP.Core.Models.GitRepository;

namespace CMP.Functions.Services
{
    public class AzureDevOpsGitRepositoryService : IGitRepositoryService
    {
        private readonly ILogger<AzureDevOpsGitRepositoryService> _logger;
        private readonly GitRepositoryOptions _options;

        public AzureDevOpsGitRepositoryService(
            IOptions<GitRepositoryOptions> options, 
            ILogger<AzureDevOpsGitRepositoryService> logger)
        {
            _logger = logger;
            _options = options.Value;
        }
        public async Task<CMPGitRepository> GetRepository()
        {
            var creds = new VssBasicCredential(string.Empty, _options.PersonalAccessToken);

            // Connect to Azure DevOps Services
            var connection = new VssConnection(new Uri(_options.GetOrganizationUri()), creds);

            // Get a GitHttpClient to talk to the Git endpoints
            using var gitClient = connection.GetClient<GitHttpClient>();

            // Get data about a specific repository
            var repo = await gitClient.GetRepositoryAsync(_options.Project, _options.Repository);

            
            return new CMPGitRepository
            {
                Name = repo.Name,
                Id = repo.Url,
                WebUrl = repo.WebUrl,
                RemoteUrl = repo.RemoteUrl
            };
        }
    }
}
