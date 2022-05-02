using CMP.Core.Models;

namespace CMP.Infrastructure.Search
{
    public interface ISearchService<T>
    {
        public Task<SearchQuery<T>> SearchAsync(SearchQuery<T> query);
    }
}
