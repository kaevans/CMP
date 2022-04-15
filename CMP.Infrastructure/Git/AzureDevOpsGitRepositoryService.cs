using CMP.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading.Tasks;
using CMPGitRepository = CMP.Core.Models.GitRepository;

namespace CMP.Infrastructure.Git
{
    public class AzureDevOpsGitRepositoryService : IGitRepository
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

            _logger.LogInformation("Retrieving repository from Azure DevOps");
            // Get data about a specific repository
            var repo = await gitClient.GetRepositoryAsync(_options.Project, _options.Repository);

            _logger.LogInformation("Name:{0}, WebURL:{1}, URL:{2}", repo.Name, repo.WebUrl, repo.Url);
            return new CMPGitRepository
            {
                Name = repo.Name,
                Id = repo.Url,
                WebUrl = repo.WebUrl,
                RemoteUrl = repo.RemoteUrl,
                Url = repo.Url
            };
        }


        public async Task<DeploymentTemplate> GetDeploymentTemplate(CMPGitRepository repo, string path)
        {
            var creds = new VssBasicCredential(string.Empty, _options.PersonalAccessToken);

            // Connect to Azure DevOps Services
            var connection = new VssConnection(new Uri(_options.GetOrganizationUri()), creds);

            // Get a GitHttpClient to talk to the Git endpoints
            using var gitClient = connection.GetClient<GitHttpClient>();

            _logger.LogInformation("Retrieving items from Azure DevOps repository");
            _logger.LogInformation("ID:{0}, Name:{1}, WebURL:{2}, URL:{3}", repo.Id, repo.Name, repo.WebUrl, repo.Url);

            var item = await gitClient.GetItemAsync(repo.Id, path + "/readme.md");
            var content = item.Content;

            //TODO: Parse content to look for description, name, URL, and image URL

            var ret = new DeploymentTemplate
            {
                Description = "",
                Id = item.ObjectId,
                Name = "",
                ImageUrl = "",
                Url = ""
            };

            //TODO: Parse data
            return ret;

        }
    }
}
