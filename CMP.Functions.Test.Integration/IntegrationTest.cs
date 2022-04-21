using CMP.Functions.Tests.Core;
using CMP.Infrastructure.Data;
using CMP.Infrastructure.Git;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;

namespace CMP.Functions.Test.Integration
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
            services.Configure<GitRepoOptions>(configurationRoot.GetSection(Infrastructure.Git.GitRepoOptions.SectionName));
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
        public void TestFunctionReadsRepositoryAndUpdatesDatabase()
        {
            dynamic requestBody = new ExpandoObject();
            dynamic resource = new ExpandoObject();
            resource.pullRequestId = "1";
            resource.pullRequestTitle = "demo title";
            dynamic repository = new ExpandoObject();
            repository.Id = "1";
            repository.Name = "demo repo";
            requestBody.resource = resource;
            requestBody.resource.repository = repository;
                       
            var mockCosmosLogger = TestingHelper.GetLogger<DeploymentTemplateRepository>();
            var cosmosService = new DeploymentTemplateRepository(CosmosOptions, mockCosmosLogger.Object);

            var mockGitLogger = TestingHelper.GetLogger<ADORepoRepository>();
            var gitService = new ADORepoRepository(GitRepoOptions, mockGitLogger.Object);

            var mockFactoryLogger = TestingHelper.GetLogger<ADODeploymentTemplateRepositoryFactory>();

            var gitItemFactory = new ADODeploymentTemplateRepositoryFactory(mockFactoryLogger.Object, GitRepoOptions);
            var mockFunctionLogger = TestingHelper.GetLogger<RepoEventsFunction>();
            var functionUnderTest = new RepoEventsFunction(mockFunctionLogger.Object, gitService,gitItemFactory, cosmosService);

            var mockRequest = TestingHelper.CreateMockRequest(requestBody);
            var ret = functionUnderTest.Run(mockRequest.Object).GetAwaiter().GetResult();

            Assert.IsTrue(ret is OkResult);


        }
    }
}