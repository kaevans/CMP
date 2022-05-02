using CMP.Core.Models.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace CMP.Infrastructure.Cloud.Azure.ResourceManagement
{

    public static class AzureServiceServiceExtensions
    {
        public static void AddAzureService(this IServiceCollection services, IConfiguration configuration)
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddHttpClient<ICloudService, ResourceManagementService>();
        }
    }

    public class ResourceManagementService : IResourceManagementService
    {        
        private readonly HttpClient _httpClient;        
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly ResourceManagementOptions _options;
        private readonly ILogger<ResourceManagementService> _logger;

        public ResourceManagementService(
            ITokenAcquisition tokenAcquisition, 
            HttpClient httpClient, 
            IOptions<ResourceManagementOptions> options, 
            ILogger<ResourceManagementService> logger)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;                     
            _options = options.Value;
            _logger = logger;
        }

        private async Task PrepareAuthenticatedClient()
        {
            //You would specify the scopes (delegated permissions) here for which you desire an Access token of this API from Azure AD.
            //Note that these scopes can be different from what you provided in startup.cs.
            //The scopes provided here can be different or more from the ones provided in Startup.cs. Note that if they are different,
            //then the user might be prompted to consent again.
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "https://management.core.windows.net/user_impersonation" });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsAsync()
        {
            await PrepareAuthenticatedClient();
                        
            var response = await _httpClient.GetAsync($"{_options.APIEndpoint }subscriptions?api-version={ _options.APIVersion}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                var subscriptionJson = JsonConvert.DeserializeObject<SubscriptionJson>(content);

                return subscriptionJson.Subscriptions;
            }

            _logger.LogError($"Unexpected status code: {response.StatusCode}:{response.Content}");
            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<IEnumerable<ResourceGroup>> GetResourceGroupsAsync(string subscriptionId)
        {
            await PrepareAuthenticatedClient();

            var response = await _httpClient.GetAsync($"{_options.APIEndpoint }subscriptions/{subscriptionId}/resourceGroups?api-version={ _options.APIVersion}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
               try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var resourceGroupJson = JsonConvert.DeserializeObject<ResourceGroupJson>(content);

                    return resourceGroupJson.ResourceGroups;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
    }
}
