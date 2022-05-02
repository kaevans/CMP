using Azure.Search.Documents.Models;

namespace CMP.Core.Models
{
    public class SearchQuery<T>
    {
        public string SearchText { get; set; }

        public SearchResults<T> ResultList;
    }
}
