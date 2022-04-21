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
    public class RepoUpdateFunction
    {
        private readonly ILogger<RepoUpdateFunction> _logger;        
        private readonly IGitRepoRepository _gitRepositoryService;
        private readonly ICosmosDbRepository<DeploymentTemplate> _cosmosDbRepositoryService;
        private readonly IGitRepoItemRepositoryFactory<DeploymentTemplate> _gitRepoItemRepositoryFactory;

        public RepoUpdateFunction(ILogger<RepoUpdateFunction> logger, 
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
        
        [FunctionName("RepoUpdate")]
        public async Task Run(
            [TimerTrigger("0 0 */5 * * *")] TimerInfo myTimer)
        {
            try
            {                
                _logger.LogInformation("Timer trigger - Executed: {0}, Schedule:{1}, Next:{2}", DateTime.Now, myTimer.Schedule.ToString(), myTimer.Schedule.GetNextOccurrence(DateTime.Now).ToLocalTime());                

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
                
                
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());                
            }
        }
    }
}
