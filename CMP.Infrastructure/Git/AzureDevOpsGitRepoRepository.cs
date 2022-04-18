using CMP.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using CMPGitRepository = CMP.Core.Models.GitRepository;

namespace CMP.Infrastructure.Git
{
    public class AzureDevOpsGitRepoRepository : IGitRepoRepository<DeploymentTemplate>
    {
        private readonly ILogger<AzureDevOpsGitRepoRepository> _logger;
        private readonly GitRepoOptions _options;

        public AzureDevOpsGitRepoRepository(
            IOptions<GitRepoOptions> options,
            ILogger<AzureDevOpsGitRepoRepository> logger)
        {
            _logger = logger;
            _options = options.Value;
        }
        public async Task<CMPGitRepository> GetRepositoryAsync()
        {
            var creds = new VssBasicCredential(string.Empty, _options.PersonalAccessToken);

            // Connect to Azure DevOps Services
            var connection = new VssConnection(new Uri(_options.GetOrganizationUri()), creds);

            // Get a GitHttpClient to talk to the Git endpoints
            using var gitClient = connection.GetClient<GitHttpClient>();

            _logger.LogInformation("Retrieving repository from Azure DevOps");
            // Get data about a specific repository
            var repo = await gitClient.GetRepositoryAsync(_options.Project, _options.Repository);

            _logger.LogInformation("ID:{0}, Name:{1}, WebURL:{2}, URL:{3}", repo.Id, repo.Name, repo.WebUrl, repo.Url);
            return new CMPGitRepository
            {
                Id = repo.Id.ToString(),
                Name = repo.Name,                
                WebUrl = repo.WebUrl,
                RemoteUrl = repo.RemoteUrl,
                Url = repo.Url
            };
        }        

        public async Task<IEnumerable<DeploymentTemplate>> GetItemsAsync()
        {
            var ret = new List<DeploymentTemplate>();

            var repo = await GetRepositoryAsync();

            var creds = new VssBasicCredential(string.Empty, _options.PersonalAccessToken);

            // Connect to Azure DevOps Services
            var connection = new VssConnection(new Uri(_options.GetOrganizationUri()), creds);

            // Get a GitHttpClient to talk to the Git endpoints
            using var gitClient = connection.GetClient<GitHttpClient>();

            _logger.LogInformation("Retrieving items from Azure DevOps");
            
            var items = await gitClient.GetItemsAsync(
                Guid.Parse(repo.Id),
                scopePath: "/",
                recursionLevel: VersionControlRecursionType.Full,                 
                includeContentMetadata: true);

            var metadataFiles = items
                    .Where(i => i.Path.Contains(
                        _options.MetadataFile, 
                        StringComparison.OrdinalIgnoreCase));

            _logger.LogInformation("Found {0} metadata files", metadataFiles.Count());

            foreach (var metadataFile in metadataFiles)
            {
                _logger.LogInformation("Retrieving:{0}, URL:{1}", metadataFile.Path, metadataFile.Url);

                var item = await gitClient.GetItemAsync(
                    Guid.Parse(repo.Id),
                    metadataFile.Path,
                    includeContent:true,                    
                    includeContentMetadata: true);

                _logger.LogInformation("Deserializing:{0}", item.Content);
                
                var deploymentTemplate = JsonConvert.DeserializeObject<DeploymentTemplate>(item.Content);

                var readmePath = item.Path.Replace(_options.MetadataFile, "readme.md", StringComparison.OrdinalIgnoreCase);
                if(null != deploymentTemplate)
                {
                    deploymentTemplate.Id = item.ObjectId;
                    deploymentTemplate.Url = item.Url;
                    deploymentTemplate.Path = item.Path;
                    deploymentTemplate.ReadmeUrl = String.Format("{0}/{1}/_git/{2}?path={3}", _options.GetOrganizationUri(), _options.Project, _options.Repository, readmePath);
                    ret.Add(deploymentTemplate);
                }
                else
                {
                    _logger.LogWarning("Unable to deserialize:{0}", item.Content);
                }
                
            }

            return ret;
        }

        public Task<IEnumerable<DeploymentTemplate>> GetItemsAsync(string query)
        {
            throw new NotImplementedException();
        }

        public Task<DeploymentTemplate> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<DeploymentTemplate> AddAsync(DeploymentTemplate entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(DeploymentTemplate entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(DeploymentTemplate entity)
        {
            throw new NotImplementedException();
        }
    }
}
