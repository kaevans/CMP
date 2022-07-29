using Azure.Core;
using Azure.Identity;
using CMP.Core.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CMP.Infrastructure.CosmosDb
{

    public abstract class CosmosDbRepository<T> : ICosmosDbRepository<T> where T : Entity
    {
        private readonly ILogger<CosmosDbRepository<T>> _logger;
        private readonly CosmosDbOptions _options;
        private readonly Container _container;


        public CosmosDbRepository(
            IOptions<CosmosDbOptions> options,
            ILogger<CosmosDbRepository<T>> logger
            )
        {
            _logger = logger;
            _options = options.Value;

            var cred = new DefaultAzureCredential(
                    new DefaultAzureCredentialOptions { ExcludeInteractiveBrowserCredential = true });
            
            var dbClient = new CosmosClient(
                options.Value.Account, cred);            
            _container = dbClient.GetContainer(options.Value.DatabaseName, options.Value.ContainerName);
        }

        public async Task<T> AddAsync(T entity)
        {
            _logger.LogInformation("CreateItemAsync:{0}", entity.Id);
            await this._container.CreateItemAsync<T>(entity, new PartitionKey(entity.Id));
            return (T)entity;
        }


        public async Task DeleteAsync(T entity)
        {            
            await this.DeleteAsync(entity.Id);            
        }

        public async Task DeleteAsync(string id)
        {
            _logger.LogInformation("DeleteAsync:{0}", id);
            await this._container.DeleteItemAsync<T>(id, new PartitionKey(id));
        }

        public async Task<T> GetByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("GetByIdAsync:{0}", id);
                ItemResponse<T> response = await this._container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(string queryString)
        {
            _logger.LogInformation("GetItemsAsync:{0}", queryString);
            var query = this._container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }
            _logger.LogInformation("Item count:{0}", results.Count);
            return results;
        }

        public async Task<IEnumerable<T>> GetItemsAsync()
        {
            _logger.LogInformation("GetItemsAsync");
            var query = this._container.GetItemQueryIterator<T>(
                queryDefinition: null);
            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }
            _logger.LogInformation("Item count:{0}", results.Count);
            return results;
        }

        public async Task UpdateAsync(T entity)
        {
            _logger.LogInformation("UpdateAsync:{0}", entity.Id);
            await this._container.UpsertItemAsync<T>(entity, new PartitionKey(entity.Id));
        }

    }

}
