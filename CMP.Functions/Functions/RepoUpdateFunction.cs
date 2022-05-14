using CMP.Core.Models;
using CMP.Infrastructure.CosmosDb;
using CMP.Infrastructure.Git;
using CMP.Infrastructure.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
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
        private readonly IAzureBlobStorageRepository _blobStorageRepository;

        public RepoUpdateFunction(ILogger<RepoUpdateFunction> logger, 
            IGitRepoRepository gitRepositoryService,
            IGitRepoItemRepositoryFactory<DeploymentTemplate> gitRepoItemRepositoryFactory,
            ICosmosDbRepository<DeploymentTemplate> cosmosDbRepositoryService,
            IAzureBlobStorageRepository blobStorageRepository
            )
        {
            _logger = logger;
            _gitRepositoryService = gitRepositoryService;
            _cosmosDbRepositoryService = cosmosDbRepositoryService;
            _gitRepoItemRepositoryFactory = gitRepoItemRepositoryFactory;
            _blobStorageRepository = blobStorageRepository;
        }
        
        [FunctionName("RepoUpdate")]
        public async Task Run(
            [TimerTrigger("%Schedule%")] TimerInfo myTimer)
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
                        if(null != item.ArchitectureDiagramContents)
                        {
                            using(item.ArchitectureDiagramContents)
                            {
                                await _blobStorageRepository.AddAsync(item.Id, item.ArchitectureDiagramContents);
                                //Write the new blob URL to the item
                                item.ArchitectureDiagramUri = await _blobStorageRepository.GetSasUri(item.Id);
                            }                            
                        }
                        
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
