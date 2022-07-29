using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP.Core.Models
{
    public class GitRepoItem : Entity
    {
        [JsonProperty(PropertyName = "$schema")]
        public string Schema { get; set; }

        [JsonProperty(PropertyName = "commitId")]
        public string CommitId { get; set; }

        [JsonProperty(PropertyName = "originalObjectId")]
        public string OriginalObjectId { get; set; }

        [JsonProperty(PropertyName = "gitUsername")]
        public string GitUserName { get; set; }

        [JsonProperty(PropertyName = "dateUpdated")]
        public string DateUpdated { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }
    }
}
