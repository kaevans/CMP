using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP.Infrastructure.Storage
{
    public class AzureBlobStorageOptions
    {
        public const string SectionName = "CosmosDb";

        public string ConnectionString { get; set; }
        

    }
}
