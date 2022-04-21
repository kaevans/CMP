using CMP.Core.Models;
using CMP.Infrastructure.Data;
using CMP.Infrastructure.Git;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace CMP.Functions
{
    public class RepoEventsFunction
    {
        private readonly ILogger<RepoEventsFunction> _logger;        
        private readonly IGitRepoRepository _gitRepositoryService;
        private readonly ICosmosDbRepository<DeploymentTemplate> _cosmosDbRepositoryService;
        private readonly IGitRepoItemRepositoryFactory<DeploymentTemplate> _gitRepoItemRepositoryFactory;

        public RepoEventsFunction(ILogger<RepoEventsFunction> logger, 
            IGitRepoRepository gitRepositoryService,
            IGitRepoItemRepositoryFactory<DeploymentTemplate> gitRepoItemRepositoryFactory,
            ICosmosDbRepository<DeploymentTemplate> cosmosDbRepositoryService
            )
        {
            _logger = logger;
            _gitRepositoryService = gitRepositoryService;
            _cosmosDbRepositoryService = cosmosDbRepositoryService;
            _gitRepoItemRepositoryFactory = gitRepoItemRepositoryFactory;
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

                // Get all repositories from project                
                var repoList = await _gitRepositoryService.GetItemsAsync();

                foreach (var repo in repoList)
                {
                    var repoItemRepository = _gitRepoItemRepositoryFactory.GetRepoItemRepository(repo, _logger);
                    var items = await repoItemRepository.GetItemsAsync();
                    foreach (var item in items)
                    {
                        await _cosmosDbRepositoryService.UpdateAsync(item);
                    }                    
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
