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

namespace CMP.Web.Tests.Integration
{
    [TestClass]
    public class DeploymentControllerIntegrationTest
    {
        private static IOptions<GitRepoOptions> GitRepoOptions { get; set; }
        private static IOptions<CosmosDbOptions> CosmosOptions { get; set; }
        private static IOptions<AzureBlobStorageOptions> BlobStorageOptions { get; set; }
        private static IOptions<AzureSearchOptions> SearchOptions { get; set; }

        [ClassInitialize]
        public static void Init(TestContext testContext)
        {
            var services = new ServiceCollection();

            // IOption configuration injection
            services.AddOptions();

            var configurationRoot = TestingHelper.GetIConfigurationRoot(testContext.DeploymentDirectory);
            services.Configure<GitRepoOptions>(configurationRoot.GetSection(Infrastructure.Git.GitRepoOptions.SectionName));
            services.Configure<CosmosDbOptions>(configurationRoot.GetSection(CosmosDbOptions.SectionName));
            services.Configure<AzureBlobStorageOptions>(configurationRoot.GetSection(AzureBlobStorageOptions.SectionName));
            services.Configure<AzureSearchOptions>(configurationRoot.GetSection(AzureSearchOptions.SectionName));

            var serviceProvider = services.BuildServiceProvider();

            // to use (or store in )
            GitRepoOptions = serviceProvider.GetRequiredService<IOptions<GitRepoOptions>>();
            CosmosOptions = serviceProvider.GetRequiredService<IOptions<CosmosDbOptions>>();
            BlobStorageOptions = serviceProvider.GetRequiredService<IOptions<AzureBlobStorageOptions>>();
            SearchOptions = serviceProvider.GetRequiredService<IOptions<AzureSearchOptions>>();

            var client = new Microsoft.Azure.Cosmos.CosmosClient(CosmosOptions.Value.Account, CosmosOptions.Value.Key);
            var database = client.CreateDatabaseIfNotExistsAsync(CosmosOptions.Value.DatabaseName).GetAwaiter().GetResult();
            database.Database.CreateContainerIfNotExistsAsync(CosmosOptions.Value.ContainerName, "/id").GetAwaiter().GetResult();
        }
        [TestMethod]
        public void IndexExecutesWithoutException()
        {
            var mockCosmosLogger = TestingHelper.GetLogger<DeploymentTemplateRepository>();
            var cosmosService = new DeploymentTemplateRepository(CosmosOptions, mockCosmosLogger.Object);

            var mockGitLogger = TestingHelper.GetLogger<ADORepoRepository>();
            var gitService = new ADORepoRepository(GitRepoOptions, mockGitLogger.Object);            

            var mockStorageLogger = TestingHelper.GetLogger<AzureBlobStorageRepository>();
            var storageService = new AzureBlobStorageRepository(BlobStorageOptions, mockStorageLogger.Object);

            var mockSearchLogger = TestingHelper.GetLogger<DeploymentTemplateSearchService>();
            var searchService = new DeploymentTemplateSearchService(SearchOptions, mockSearchLogger.Object);

            var mockControllerLogger = TestingHelper.GetLogger<DeploymentTemplateController>();

            var controller = new DeploymentTemplateController(mockControllerLogger.Object, GitRepoOptions, gitService, cosmosService, searchService);
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

            var mockGitLogger = TestingHelper.GetLogger<ADORepoRepository>();
            var gitService = new ADORepoRepository(GitRepoOptions, mockGitLogger.Object);            

            var mockSearchLogger = TestingHelper.GetLogger<DeploymentTemplateSearchService>();
            var searchService = new DeploymentTemplateSearchService(SearchOptions, mockSearchLogger.Object);

            var mockControllerLogger = TestingHelper.GetLogger<DeploymentTemplateController>();

            var controller = new DeploymentTemplateController(mockControllerLogger.Object, GitRepoOptions, gitService, cosmosService, searchService);
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

            var mockGitLogger = TestingHelper.GetLogger<ADORepoRepository>();
            var gitService = new ADORepoRepository(GitRepoOptions, mockGitLogger.Object);            

            var mockSearchLogger = TestingHelper.GetLogger<DeploymentTemplateSearchService>();
            var searchService = new DeploymentTemplateSearchService(SearchOptions, mockSearchLogger.Object);

            var mockControllerLogger = TestingHelper.GetLogger<DeploymentTemplateController>();

            var controller = new DeploymentTemplateController(mockControllerLogger.Object, GitRepoOptions, gitService, cosmosService, searchService);

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