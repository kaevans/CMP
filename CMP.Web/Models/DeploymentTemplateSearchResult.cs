using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace CMP.Web.Models
{
    public partial class DeploymentTemplateSearchResult
    {
        [SimpleField(IsFilterable = true, IsKey = true)]
        public string id { get; set; }

        [SimpleField()]
        public string schema { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public string type { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]        
        public string itemDisplayName { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]        
        public string description { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string summary { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public string gitUserName { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public string dateUpdated { get; set; }

        [SimpleField]
        public string url { get; set; }

        [SimpleField]
        public string path { get; set; }

        [SimpleField]
        public string readmeurl { get; set; }
    }
}
