using CMP.Core.Interfaces;
using CMP.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CMP.Infrastructure.Git
{    
    public class ADORepoRepository : IGitRepoRepository
    {
        private readonly ILogger<ADORepoRepository> _logger;
        private readonly GitRepoOptions _options;

        public ADORepoRepository(
            IOptions<GitRepoOptions> options,
            ILogger<ADORepoRepository> logger)
        {
            _logger = logger;
            _options = options.Value;
        }

        public Task<GitRepo> AddAsync(GitRepo entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(GitRepo entity)
        {
            throw new NotImplementedException();
        }

        public async Task<GitRepo> GetByIdAsync(string id)
        {
            if(!Guid.TryParse(id, out var repoIdGuid))
            {
                throw new ArgumentException("id parameter must contain a Guid string", nameof(id));
            }
                        
            var creds = new VssBasicCredential(string.Empty, _options.PersonalAccessToken);

            // Connect to Azure DevOps Services
            var connection = new VssConnection(new Uri(_options.GetOrganizationUri()), creds);

            // Get a GitHttpClient to talk to the Git endpoints
            using var gitClient = connection.GetClient<GitHttpClient>();

            _logger.LogInformation("Retrieving repository for project {0} from Azure DevOps", _options.Project);
            // Get data about a specific repository
            var repo = await gitClient.GetRepositoryAsync(repoIdGuid);

            _logger.LogInformation("Name:{0}, Id:{1}, Url:{2}, WebUrl:{3}", repo.Name, repo.Id, repo.Url, repo.WebUrl);

            var ret = new GitRepo
            {
                Id = repo.Id.ToString(),
                Name = repo.Name,
                WebUrl = repo.WebUrl,
                RemoteUrl = repo.RemoteUrl,
                Url = repo.Url
            };

            return ret;
        }


        public async Task<IEnumerable<GitRepo>> GetItemsAsync(string repoName)
        {
            var ret = new List<GitRepo>();
            var creds = new VssBasicCredential(string.Empty, _options.PersonalAccessToken);

            // Connect to Azure DevOps Services
            var connection = new VssConnection(new Uri(_options.GetOrganizationUri()), creds);

            // Get a GitHttpClient to talk to the Git endpoints
            using var gitClient = connection.GetClient<GitHttpClient>();

            _logger.LogInformation("Retrieving repository for project {0} from Azure DevOps", _options.Project);
            // Get data about a specific repository
            var repo = await gitClient.GetRepositoryAsync(_options.Project, repoName);

            _logger.LogInformation("Name:{0}, Id:{1}, Url:{2}, WebUrl:{3}", repo.Name, repo.Id, repo.Url, repo.WebUrl);

            ret.Add(new GitRepo
            {
                Id = repo.Id.ToString(),
                Name = repo.Name,
                WebUrl = repo.WebUrl,
                RemoteUrl = repo.RemoteUrl,
                Url = repo.Url
            });
            
            return ret;
        }

        public Task UpdateAsync(GitRepo entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<GitRepo>> GetItemsAsync()
        {
            var ret = new List<GitRepo>();
            var creds = new VssBasicCredential(string.Empty, _options.PersonalAccessToken);

            // Connect to Azure DevOps Services
            var connection = new VssConnection(new Uri(_options.GetOrganizationUri()), creds);

            // Get a GitHttpClient to talk to the Git endpoints
            using var gitClient = connection.GetClient<GitHttpClient>();

            _logger.LogInformation("Retrieving repository for project {0} from Azure DevOps", _options.Project);
            // Get data about a specific repository
            var repoList = await gitClient.GetRepositoriesAsync(_options.Project);

            _logger.LogInformation("Count of repositories found:{0}", repoList.Count);
            
            foreach (var repo in repoList)
            {
                ret.Add(new GitRepo
                {
                    Id = repo.Id.ToString(),
                    Name = repo.Name,
                    WebUrl = repo.WebUrl,
                    RemoteUrl = repo.RemoteUrl,
                    Url = repo.Url
                });
            }

            return ret;
        }
    }
}
