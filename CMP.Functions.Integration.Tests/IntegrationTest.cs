
using CMP.Functions.Options;
using CMP.Functions.Services;
using CMP.Functions.Tests.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CMPGitRepository = CMP.Core.Models.GitRepository;

namespace CMP.Functions.Integration.Tests
{
    [TestClass]
    public class IntegrationTest
    {
        private static IOptions<GitRepositoryOptions> OptionsConfig { get; set; }

        [ClassInitialize]
        public static void Init(TestContext testContext)
        {
            var services = new ServiceCollection();

            // IOption configuration injection
            services.AddOptions();

            var configurationRoot = TestingHelper.GetIConfigurationRoot(testContext.DeploymentDirectory);
            services.Configure<GitRepositoryOptions>(configurationRoot.GetSection(GitRepositoryOptions.SectionName));


            var serviceProvider = services.BuildServiceProvider();

            // to use (or store in )
            OptionsConfig = serviceProvider.GetRequiredService<IOptions<GitRepositoryOptions>>();
        }

        [TestMethod]
        public void TestServiceReturnsValidRepo()
        {
            var logger = TestingHelper.GetLogger<AzureDevOpsGitRepositoryService>();
            var service = new AzureDevOpsGitRepositoryService(OptionsConfig, logger.Object);

            var expected = new CMPGitRepository
            {
                Id = "0",
                Name = OptionsConfig.Value.Repository
            };

            var result = service.GetRepository().GetAwaiter().GetResult();


            Assert.IsTrue(result is CMPGitRepository);
            Assert.IsNotNull(result);


            Assert.AreEqual(result.Name, expected.Name);

        }

        [TestMethod]
        public void TestOptionsReturnsValidUri()
        {

            var optionObject = OptionsConfig.Value;

            var expected = string.Format(optionObject.CollectionUri, optionObject.Organization);
            var actual = OptionsConfig.Value.GetOrganizationUri();

            Assert.AreEqual(expected, actual);
        }
    }
}