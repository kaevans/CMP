using CMP.Core.Interfaces;
using CMP.Core.Models;

namespace CMP.Infrastructure.Storage
{
    public interface IAzureBlobStorageRepository
    {
        Task<Stream> GetByIdAsync(string id);
        Task AddAsync(string id, Stream blobContents);
        Task DeleteAsync(string id);
        Uri GetBlobUri(string id);
        Task<Uri> GetSasUri(string id);
    }
}
