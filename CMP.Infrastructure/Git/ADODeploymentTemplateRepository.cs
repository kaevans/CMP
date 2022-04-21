using CMP.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;

namespace CMP.Infrastructure.Git
{

    public class ADODeploymentTemplateRepository : IGitRepoItemRepository<DeploymentTemplate>
    {
        private readonly GitRepo _repo;
        private readonly GitRepoOptions _options;
        private readonly ILogger _logger;

        public ADODeploymentTemplateRepository(GitRepo repo, GitRepoOptions options, ILogger logger)
        {
            _options = options;
            _repo = repo;
            _logger = logger;
        }

        public Task<DeploymentTemplate> AddAsync(DeploymentTemplate entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(DeploymentTemplate entity)
        {
            throw new NotImplementedException();
        }

        public Task<DeploymentTemplate> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DeploymentTemplate>> GetItemsAsync()
        {
            var ret = new List<DeploymentTemplate>();            

            var creds = new VssBasicCredential(string.Empty, _options.PersonalAccessToken);

            // Connect to Azure DevOps Services
            var connection = new VssConnection(new Uri(_options.GetOrganizationUri()), creds);

            // Get a GitHttpClient to talk to the Git endpoints
            using var gitClient = connection.GetClient<GitHttpClient>();

            _logger.LogInformation("Retrieving items from Azure DevOps repo: {0}", _repo.Id);

            var items = await gitClient.GetItemsAsync(
                Guid.Parse(_repo.Id),
                scopePath: "/",
                recursionLevel: VersionControlRecursionType.Full,
                includeContentMetadata: false);

            _logger.LogInformation("Returned {0} items from Azure DevOps repo: {1}", items.Count(), _repo.Id);

            var metadataFiles = items
                    .Where(i => i.Path.Contains(
                        _options.MetadataFile,
                        StringComparison.OrdinalIgnoreCase));

            _logger.LogInformation("Returned {0} files containing path {1} from Azure DevOps repo: {2}", metadataFiles.Count(), _options.MetadataFile, _repo.Id);

            foreach (var metadataFile in metadataFiles)
            {
                _logger.LogInformation("Retrieving:{0}, URL:{1}", metadataFile.Path, metadataFile.Url);

                var item = await gitClient.GetItemAsync(
                    Guid.Parse(_repo.Id),
                    metadataFile.Path,
                    includeContent: true,
                    includeContentMetadata: true);

                _logger.LogInformation("Deserializing:{0}", item.Content);

                var deploymentTemplate = JsonConvert.DeserializeObject<DeploymentTemplate>(item.Content);

                var readmePath = item.Path.Replace(_options.MetadataFile, "readme.md", StringComparison.OrdinalIgnoreCase);
                if (null != deploymentTemplate)
                {
                    deploymentTemplate.Id = item.ObjectId;
                    deploymentTemplate.Url = item.Url;
                    deploymentTemplate.Path = item.Path;
                    deploymentTemplate.ReadmeUrl = String.Format("{0}/{1}/_git/{2}?path={3}", _options.GetOrganizationUri(), _options.Project, _repo.Name, readmePath);
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

        public Task UpdateAsync(DeploymentTemplate entity)
        {
            throw new NotImplementedException();
        }
    }
}
