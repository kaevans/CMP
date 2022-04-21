using CMP.Core.Interfaces;
using CMP.Core.Models;
using CMP.Functions.Tests.Core;
using CMP.Infrastructure.Data;
using CMP.Infrastructure.Git;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics;
using System.Dynamic;

namespace CMP.Functions.Tests
{
    [TestClass]
    public class MockTest
    {
        private static IOptions<GitRepoOptions> OptionsConfig { get; set; }

        [ClassInitialize]
        public static void Init(TestContext testContext)
        {            
            var services = new ServiceCollection();

            // IOption configuration injection
            services.AddOptions();

            var configurationRoot = TestingHelper.GetIConfigurationRoot(testContext.DeploymentDirectory);
            services.Configure<GitRepoOptions>(configurationRoot.GetSection(GitRepoOptions.SectionName));
            

            var serviceProvider = services.BuildServiceProvider();

            // to use (or store in )
            OptionsConfig = serviceProvider.GetRequiredService<IOptions<GitRepoOptions>>();

            Trace.WriteLine(OptionsConfig.Value.Organization);
        }

        [TestMethod]
        public void TestFunctionExecutesWithoutExceptions()
        {
            var mockService = new Mock<IGitRepoRepository>();

            var mockCosmosDbService = new Mock<ICosmosDbRepository<DeploymentTemplate>>();
                                                                 
            var mockLoggerFunction = TestingHelper.GetLogger<RepoUpdateFunction>();

            var mockFactory = new Mock<IGitRepoItemRepositoryFactory<DeploymentTemplate>>();
            var functionUnderTest = new RepoUpdateFunction(mockLoggerFunction.Object, mockService.Object, mockFactory.Object, mockCosmosDbService.Object);
            
            TimerSchedule schedule = new DailySchedule("2:00:00");
            TimerInfo timerInfo = new TimerInfo(schedule, It.IsAny<ScheduleStatus>(), false);

            functionUnderTest.Run(timerInfo).GetAwaiter().GetResult();                                  
        }

    }
}
