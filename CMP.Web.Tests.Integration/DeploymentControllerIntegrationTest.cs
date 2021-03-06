using CMP.Tests.Core;
using CMP.Web.Controllers;
using CMP.Infrastructure.CosmosDb;
using CMP.Infrastructure.Git;
using CMP.Infrastructure.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Moq;
using CMP.Infrastructure.Search;
using CMP.Web.Services;
using CMP.Core.Models;
using CMP.Web.Models;
using Azure.Identity;

namespace CMP.Web.Tests.Integration
{
    [TestClass]
    public class DeploymentControllerIntegrationTest
    {
        private static IOptions<CosmosDbOptions> CosmosOptions { get; set; }
        private static IOptions<AzureSearchOptions> SearchOptions { get; set; }

        [ClassInitialize]
        public static void Init(TestContext testContext)
        {
            var services = new ServiceCollection();

            // IOption configuration injection
            services.AddOptions();

            var configurationRoot = TestingHelper.GetIConfigurationRoot(testContext.DeploymentDirectory);            
            services.Configure<CosmosDbOptions>(configurationRoot.GetSection(CosmosDbOptions.SectionName));            
            services.Configure<AzureSearchOptions>(configurationRoot.GetSection(AzureSearchOptions.SectionName));

            var serviceProvider = services.BuildServiceProvider();

            // to use (or store in )            
            CosmosOptions = serviceProvider.GetRequiredService<IOptions<CosmosDbOptions>>();            
            SearchOptions = serviceProvider.GetRequiredService<IOptions<AzureSearchOptions>>();

            var cred = new DefaultAzureCredential(
                    new DefaultAzureCredentialOptions { ExcludeInteractiveBrowserCredential = true });
            var client = new Microsoft.Azure.Cosmos.CosmosClient(CosmosOptions.Value.Account, cred);
            var database = client.CreateDatabaseIfNotExistsAsync(CosmosOptions.Value.DatabaseName).GetAwaiter().GetResult();
            database.Database.CreateContainerIfNotExistsAsync(CosmosOptions.Value.ContainerName, "/id").GetAwaiter().GetResult();
        }
        [TestMethod]
        public void IndexExecutesWithoutException()
        {
            var mockCosmosLogger = TestingHelper.GetLogger<DeploymentTemplateRepository>();
            var cosmosService = new DeploymentTemplateRepository(CosmosOptions, mockCosmosLogger.Object);

            var mockSearchLogger = TestingHelper.GetLogger<DeploymentTemplateSearchService>();
            var searchService = new DeploymentTemplateSearchService(SearchOptions, mockSearchLogger.Object);

            var mockControllerLogger = TestingHelper.GetLogger<DeploymentTemplateController>();

            var controller = new DeploymentTemplateController(mockControllerLogger.Object, cosmosService, searchService);
            var result = controller.Index().GetAwaiter().GetResult();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<DeploymentTemplate>));

            var modelItems = (IEnumerable<DeploymentTemplate>)viewResult.Model;
            Assert.IsTrue(modelItems.Count() > 0);
        }

        [TestMethod]
        public void SearchExecutesWithoutException()
        {
            var mockCosmosLogger = TestingHelper.GetLogger<DeploymentTemplateRepository>();
            var cosmosService = new DeploymentTemplateRepository(CosmosOptions, mockCosmosLogger.Object);
          

            var mockSearchLogger = TestingHelper.GetLogger<DeploymentTemplateSearchService>();
            var searchService = new DeploymentTemplateSearchService(SearchOptions, mockSearchLogger.Object);

            var mockControllerLogger = TestingHelper.GetLogger<DeploymentTemplateController>();

            var controller = new DeploymentTemplateController(mockControllerLogger.Object, cosmosService, searchService);
            var model = new SearchData { SearchText = "" };
            var result = controller.Search(model).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.Model, typeof(SearchQuery<DeploymentTemplateSearchResult>));

            var modelItems = (SearchQuery<DeploymentTemplateSearchResult>)viewResult.Model;
            Assert.IsTrue(modelItems.ResultList.TotalCount > 0);
        }


        [TestMethod]
        public void GetByIdExecutesWithoutException()
        {
            var mockCosmosLogger = TestingHelper.GetLogger<DeploymentTemplateRepository>();
            var cosmosService = new DeploymentTemplateRepository(CosmosOptions, mockCosmosLogger.Object);         

            var mockSearchLogger = TestingHelper.GetLogger<DeploymentTemplateSearchService>();
            var searchService = new DeploymentTemplateSearchService(SearchOptions, mockSearchLogger.Object);

            var mockControllerLogger = TestingHelper.GetLogger<DeploymentTemplateController>();

            var controller = new DeploymentTemplateController(mockControllerLogger.Object, cosmosService, searchService);

            //Get an item we know exists
            var allItems = cosmosService.GetItemsAsync().GetAwaiter().GetResult();
            Assert.IsNotNull(allItems);

            var expected = allItems.FirstOrDefault();

            var result = controller.GetById(expected.Id).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(DeploymentTemplate));
            var actual = (DeploymentTemplate)viewResult.Model;

            Assert.IsTrue(expected.Id == actual.Id);
            Assert.IsTrue(expected.Name == actual.Name);
            Assert.IsTrue(expected.Type == actual.Type);
            Assert.IsTrue(expected.CommitId == actual.CommitId);
            Assert.IsTrue(expected.Description == actual.Description);
            Assert.IsTrue(expected.OriginalObjectId == actual.OriginalObjectId);
            
        }

    }
}