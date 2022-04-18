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
        private readonly GitRepoOptions _options;
        private readonly IGitRepoRepository<DeploymentTemplate> _gitRepositoryService;
        private readonly ICosmosDbRepository<DeploymentTemplate> _cosmosDbRepositoryService;

        public RepoEventsFunction(ILogger<RepoEventsFunction> logger, 
            IOptions<GitRepoOptions> options,
            IGitRepoRepository<DeploymentTemplate> gitRepositoryService,
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

                // Get all items from repository                
                var items = await _gitRepositoryService.GetItemsAsync();

                foreach (var item in items)
                {
                    await _cosmosDbRepositoryService.UpdateAsync(item);
                }
                
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());

                return new BadRequestObjectResult(ex.ToString());
            }
        }
    }
}
