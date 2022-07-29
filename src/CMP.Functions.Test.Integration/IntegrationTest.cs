using CMP.Tests.Core;
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
using Azure.Identity;

namespace CMP.Functions.Test.Integration
{
    [TestClass]
    public class IntegrationTest
    {
        private static IOptions<GitRepoOptions> GitRepoOptions { get; set; }
        private static IOptions<CosmosDbOptions> CosmosOptions { get; set; }
        private static IOptions<AzureBlobStorageOptions> BlobStorageOptions { get; set; }

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

            var serviceProvider = services.BuildServiceProvider();

            // to use (or store in )
            GitRepoOptions = serviceProvider.GetRequiredService<IOptions<GitRepoOptions>>();
            CosmosOptions = serviceProvider.GetRequiredService<IOptions<CosmosDbOptions>>();
            BlobStorageOptions = serviceProvider.GetRequiredService<IOptions<AzureBlobStorageOptions>>();

            var cred = new DefaultAzureCredential(
                    new DefaultAzureCredentialOptions { ExcludeInteractiveBrowserCredential = true });

            var client = new Microsoft.Azure.Cosmos.CosmosClient(CosmosOptions.Value.Account, cred);
            var database = client.CreateDatabaseIfNotExistsAsync(CosmosOptions.Value.DatabaseName).GetAwaiter().GetResult();
            database.Database.CreateContainerIfNotExistsAsync(CosmosOptions.Value.ContainerName, "/id").GetAwaiter().GetResult();
        }

        [TestMethod]
        public void TestFunctionExecutesWithoutExceptions()
        {
                       
            var mockCosmosLogger = TestingHelper.GetLogger<DeploymentTemplateRepository>();
            var cosmosService = new DeploymentTemplateRepository(CosmosOptions, mockCosmosLogger.Object);

            var mockGitLogger = TestingHelper.GetLogger<ADORepoRepository>();
            var gitService = new ADORepoRepository(GitRepoOptions, mockGitLogger.Object);

            var mockFactoryLogger = TestingHelper.GetLogger<ADODeploymentTemplateRepositoryFactory>();

            var gitItemFactory = new ADODeploymentTemplateRepositoryFactory(mockFactoryLogger.Object, GitRepoOptions);

            var mockStorageLogger = TestingHelper.GetLogger<AzureBlobStorageRepository>();
            var storageService = new AzureBlobStorageRepository(BlobStorageOptions, mockStorageLogger.Object);

            var mockFunctionLogger = TestingHelper.GetLogger<RepoUpdateFunction>();

            var functionUnderTest = new RepoUpdateFunction(mockFunctionLogger.Object, gitService, gitItemFactory, cosmosService, storageService);

            TimerSchedule schedule = new DailySchedule("2:00:00");
            TimerInfo timerInfo = new TimerInfo(schedule, It.IsAny<ScheduleStatus>(), false);

            functionUnderTest.Run(timerInfo).GetAwaiter().GetResult();            
        }
    }
}