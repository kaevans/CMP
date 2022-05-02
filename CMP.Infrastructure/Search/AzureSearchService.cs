using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using CMP.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CMP.Infrastructure.Search
{
    public class AzureSearchService<T> : ISearchService<T>
    {
        private readonly AzureSearchOptions _azureSearchOptions;
        private readonly ILogger<AzureSearchService<T>> _logger;
        private static SearchClient _searchClient;
        private static SearchIndexClient _indexClient;
        

        public AzureSearchService(IOptions<AzureSearchOptions> azureSearchOptions, ILogger<AzureSearchService<T>> logger)
        {
            _azureSearchOptions = azureSearchOptions.Value;
            _logger = logger;

            _indexClient = new SearchIndexClient(new Uri(_azureSearchOptions.SearchServiceUri), new AzureKeyCredential(_azureSearchOptions.SearchServiceQueryApiKey));
            _searchClient = _indexClient.GetSearchClient(_azureSearchOptions.IndexName);
        }

        public async Task<SearchQuery<T>> SearchAsync(SearchQuery<T> query)
        {
            var options = new SearchOptions()
            {
                IncludeTotalCount = true
            };            
            query.ResultList = await _searchClient.SearchAsync<T>(query.SearchText, options).ConfigureAwait(false);
            return query;
        }
    }
}
