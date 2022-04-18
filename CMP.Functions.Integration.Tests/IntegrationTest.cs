
using CMP.Core.Models;

using CMP.Functions.Tests.Core;
using CMP.Infrastructure.Data;
using CMP.Infrastructure.Git;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using CMPGitRepository = CMP.Core.Models.GitRepository;

namespace CMP.Infrastructure.Tests.Integration
{
    [TestClass]
    public class IntegrationTest
    {
        private static IOptions<GitRepoOptions> GitRepoOptions { get; set; }
        private static IOptions<CosmosDbOptions> CosmosOptions { get; set; }

        [ClassInitialize]
        public static void Init(TestContext testContext)
        {
            var services = new ServiceCollection();

            // IOption configuration injection
            services.AddOptions();

            var configurationRoot = TestingHelper.GetIConfigurationRoot(testContext.DeploymentDirectory);
            services.Configure<GitRepoOptions>(configurationRoot.GetSection(Git.GitRepoOptions.SectionName));
            services.Configure<CosmosDbOptions>(configurationRoot.GetSection(CosmosDbOptions.SectionName));

            var serviceProvider = services.BuildServiceProvider();

            // to use (or store in )
            GitRepoOptions = serviceProvider.GetRequiredService<IOptions<GitRepoOptions>>();
            CosmosOptions = serviceProvider.GetRequiredService<IOptions<CosmosDbOptions>>();


            var client = new Microsoft.Azure.Cosmos.CosmosClient(CosmosOptions.Value.Account, CosmosOptions.Value.Key);
            var database = client.CreateDatabaseIfNotExistsAsync(CosmosOptions.Value.DatabaseName).GetAwaiter().GetResult();
            database.Database.CreateContainerIfNotExistsAsync(CosmosOptions.Value.ContainerName, "/id").GetAwaiter().GetResult();
        }

        [TestMethod]
        public void TestServiceReturnsValidRepo()
        {
            var logger = TestingHelper.GetLogger<AzureDevOpsGitRepoRepository>();
            var service = new AzureDevOpsGitRepoRepository(GitRepoOptions, logger.Object);

            var expected = new CMPGitRepository
            {
                Id = "0",
                Name = GitRepoOptions.Value.Repository
            };

            var result = service.GetRepositoryAsync().GetAwaiter().GetResult();


            Assert.IsTrue(result is CMPGitRepository);
            Assert.IsNotNull(result);


            Assert.AreEqual(result.Name, expected.Name);

        }

        [TestMethod]
        public void TestDevOpsReturnsReadMeItems()
        {
            var logger = TestingHelper.GetLogger<AzureDevOpsGitRepoRepository>();
            var service = new AzureDevOpsGitRepoRepository(GitRepoOptions, logger.Object);

            var items = service.GetItemsAsync().GetAwaiter().GetResult();

            Assert.IsNotNull(items);            
            Assert.IsTrue(items.Count() > 0);
        }

        [TestMethod]
        public void TestOptionsReturnsValidUri()
        {

            var optionObject = GitRepoOptions.Value;

            var expected = string.Format(optionObject.CollectionUri, optionObject.Organization);
            var actual = GitRepoOptions.Value.GetOrganizationUri();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCosmosDbReturnsValidAccount()
        {
            var logger = TestingHelper.GetLogger<DeploymentTemplateRepository>();            
            var service = new DeploymentTemplateRepository(CosmosOptions, logger.Object);           

            var expected = new Core.Models.DeploymentTemplate
            {
                Description = "testing",
                Id = Guid.NewGuid().ToString(),
                Name = "testing",
                Url = "testing",
                Path = "testing",
                ReadmeUrl = "testing",
                DateUpdated = "testing",
                GitUserName = "testing",
                Schema = "testing",
                Summary = "testing", 
                Type = "testing"
            };

            Assert.IsNotNull(service);

            service.AddAsync(expected).GetAwaiter().GetResult();

            var actual = service.GetByIdAsync(expected.Id).GetAwaiter().GetResult();

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.Url, actual.Url);
            Assert.AreEqual(expected.ReadmeUrl, actual.ReadmeUrl);
            Assert.AreEqual(expected.DateUpdated, actual.DateUpdated);
            Assert.AreEqual(expected.GitUserName, actual.GitUserName);
            Assert.AreEqual(expected.Schema, actual.Schema);
            Assert.AreEqual(expected.Summary, actual.Summary);
            Assert.AreEqual(expected.Type, actual.Type);

            //Clean up 
            service.DeleteAsync(actual).GetAwaiter().GetResult();
            
        }
    }
}