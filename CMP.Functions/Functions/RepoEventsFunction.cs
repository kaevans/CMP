using CMP.Core.Interfaces;
using CMP.Core.Models;
using CMP.Infrastructure.Data;
using CMP.Infrastructure.Git;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using CMPGitRepository = CMP.Core.Models.GitRepository;

namespace CMP.Functions
{
    public class RepoEventsFunction
    {
        private readonly ILogger<RepoEventsFunction> _logger;
        private readonly GitRepositoryOptions _options;
        private readonly IGitRepository _gitRepositoryService;
        private readonly ICosmosDbRepository<DeploymentTemplate> _cosmosDbRepositoryService;

        public RepoEventsFunction(ILogger<RepoEventsFunction> logger, 
            IOptions<GitRepositoryOptions> options,
            IGitRepository gitRepositoryService,
            ICosmosDbRepository<DeploymentTemplate> cosmosDbRepositoryService
            )
        {
            _logger = logger;
            _options = options.Value;
            _gitRepositoryService = gitRepositoryService;
            _cosmosDbRepositoryService = cosmosDbRepositoryService; 
        }


        [FunctionName("RepoEvents")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            try
            {
                _logger.LogInformation("Service Hook Received.");

                // Get request body
                var data = await req.ReadAsStringAsync();

                _logger.LogInformation("Data Received: " + data);

                // Get the pull request object from the service hooks payload
                dynamic jObject = JsonConvert.DeserializeObject(data);

                // Get the pull request id
                int pullRequestId;
                if (!Int32.TryParse(jObject.resource.pullRequestId.ToString(), out pullRequestId))
                {
                    _logger.LogWarning("Failed to parse the pull request id from the service hooks payload.");
                };

                // Get the pull request title
                string pullRequestTitle = jObject.resource.pullRequestTitle;

                _logger.LogInformation("Service Hook Received for PR: " + pullRequestId + " " + pullRequestTitle);

                var ret = new CMPGitRepository { Id = jObject.resource.repository.Id, Name = jObject.resource.repository.Name };

                _logger.LogInformation("Id:{0}, Name:{1}", ret.Id, ret.Name);

                return new OkObjectResult(ret);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());

                return new BadRequestObjectResult(ex.ToString());
            }
        }
    }
}
