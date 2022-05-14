using System.Diagnostics;

namespace CMP.Infrastructure.Storage
{
    public class AzureBlobStorageOptions
    {
        public const string SectionName = "AzureBlobStorage";

        public string BlobServiceEndpoint { get; set; }

        public string Container { get; set; }

        public bool IsDevelopment()
        {
            return Debugger.IsAttached;
        }

    }
}
