namespace CMP.Infrastructure.Search
{
    public class AzureSearchOptions
    {
        public const string SectionName = "AzureSearch";

        public string SearchServiceUri { get; set; }
        public string SearchServiceQueryApiKey { get; set; }
        public string IndexName { get; set; }
    }
}
