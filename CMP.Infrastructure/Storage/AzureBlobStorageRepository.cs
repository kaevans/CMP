using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using CMP.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System.Text;

namespace CMP.Infrastructure.Storage
{
    public class AzureBlobStorageRepository : IAzureBlobStorageRepository
    {
        private readonly ILogger _logger;
        private readonly AzureBlobStorageOptions _azureBlobStorageOptions;


        public AzureBlobStorageRepository(
            IOptions<AzureBlobStorageOptions> options,
            ILogger<AzureBlobStorageRepository> logger)
        {
            _logger = logger;
            _azureBlobStorageOptions = options.Value;
        }

        public Uri GetBlobUri(string blobName)
        {
            var url = string.Format($"{_azureBlobStorageOptions.BlobServiceEndpoint}/{_azureBlobStorageOptions.Container}/{blobName}");
            var result = new Uri(url);

            return result;
        }

        public async Task AddAsync(string id, Stream blobContents)
        {
            string message = "Blob failed to create";

            var blobUri = GetBlobUri(id);

            var cred = new DefaultAzureCredential(
                    new DefaultAzureCredentialOptions { ExcludeInteractiveBrowserCredential = true });
            var blobClient = new BlobClient(blobUri, cred);


            try
            {
                await blobClient.UploadAsync(blobContents, overwrite: true);
                message = "Blob successfully created";
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _logger.LogError(ex, message);
                throw ex;
            }
            catch (MsalUiRequiredException ex)
            {
                _logger.LogError(ex, message);
                throw ex;
            }
            catch (Exception ex)
            {
                try
                {
                    message += $". Reason - {((Azure.RequestFailedException)ex).ErrorCode}";
                    _logger.LogError(ex, message);
                }
                catch (Exception)
                {
                    message += $". Reason - {ex.Message}";
                    _logger.LogError(ex, message);
                }
            }            
        }

        public async Task DeleteAsync(string id)
        {
            string message = "Blob failed to delete";

            var blobUri = GetBlobUri(id);

            var cred = new DefaultAzureCredential(
                    new DefaultAzureCredentialOptions { ExcludeInteractiveBrowserCredential = true });

            var blobClient = new BlobClient(blobUri, cred);

            try
            {
                await blobClient.DeleteAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
                message = "Blob successfully deleted";
                _logger.LogInformation($". {id}");
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _logger.LogError(ex, message);
                throw ex;
            }
            catch (MsalUiRequiredException ex)
            {
                _logger.LogError(ex, message);
                throw ex;
            }
            catch (Exception ex)
            {
                try
                {
                    message += $". Reason - {((Azure.RequestFailedException)ex).ErrorCode}";
                    _logger.LogError(ex, message);
                }
                catch (Exception)
                {
                    message += $". Reason - {ex.Message}";
                    _logger.LogError(ex, message);
                }
            }
            
        }

        public async Task<Stream> GetByIdAsync(string id)
        {
            string message = "Failed to get Blob";

            var blobUri = GetBlobUri(id);
            var cred = new DefaultAzureCredential(
                    new DefaultAzureCredentialOptions { ExcludeInteractiveBrowserCredential = true });
            var blobClient = new BlobClient(blobUri, cred);

            var stream = new MemoryStream();
            try
            {                
                await blobClient.DownloadToAsync(stream);

                message = "Blob successfully retrieved";
                _logger.LogInformation($"{ message}. { id}");

                return stream;                
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                await stream.DisposeAsync();
                _logger.LogError(ex, message);
                throw ex;
            }
            catch (MsalUiRequiredException ex)
            {
                await stream.DisposeAsync();
                _logger.LogError(ex, message);
                throw ex;
            }
            catch (Exception ex)
            {
                await stream.DisposeAsync();
                try
                {
                    message += $". Reason - {((Azure.RequestFailedException)ex).ErrorCode}";
                    _logger.LogError(ex, message);
                    throw;
                }
                catch (Exception)
                {
                    message += $". Reason - {ex.Message}";
                    _logger.LogError(ex, message);
                    throw ex;
                }
                
            }
            
        }

        public async Task<Uri> GetSasUri(string id)
        {
            Uri result = default;

            string message = "SAS URI failed to create";

            var blobUri = GetBlobUri(id);

            var cred = new DefaultAzureCredential(
                    new DefaultAzureCredentialOptions { ExcludeInteractiveBrowserCredential = true });

            try
            {
                var blobServiceClient = new BlobServiceClient(new Uri(_azureBlobStorageOptions.BlobServiceEndpoint), cred);
                var userDelegationKey = await blobServiceClient.GetUserDelegationKeyAsync(null, DateTimeOffset.UtcNow.AddDays(5));

                var blobClient = new BlobClient(blobUri, cred);

                var sasBuilder = new BlobSasBuilder(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddDays(5))
                {
                    BlobContainerName = blobClient.BlobContainerName,
                    BlobName = blobClient.Name,
                };

                var blobUriBuilder = new BlobUriBuilder(blobClient.Uri)
                {
                    Sas = sasBuilder.ToSasQueryParameters(userDelegationKey, blobServiceClient.AccountName)
                };

                result = blobUriBuilder.ToUri();
                message = "SAS URI successfully created";
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _logger.LogError(ex, message);
                throw ex;
            }
            catch (MsalUiRequiredException ex)
            {
                _logger.LogError(ex, message);
                throw ex;
            }
            catch (Exception ex)
            {
                try
                {
                    message += $". Reason - {((Azure.RequestFailedException)ex).ErrorCode}";
                    _logger.LogError(ex, message);
                }
                catch (Exception)
                {
                    message += $". Reason - {ex.Message}";
                    _logger.LogError(ex, message);
                }
            }
            return result;
        }

    }
}
