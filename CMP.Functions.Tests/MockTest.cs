﻿using CMP.Core.Interfaces;
using CMP.Core.Models;
using CMP.Functions.Tests.Core;
using CMP.Infrastructure.Data;
using CMP.Infrastructure.Git;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics;
using System.Dynamic;
using CMPGitRepository = CMP.Core.Models.GitRepository;

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
        public void TestFunctionReturnsValidRepo()
        {
            var mockService = new Mock<IGitRepoRepository<DeploymentTemplate>>();

            dynamic requestBody = new ExpandoObject();
            dynamic resource = new ExpandoObject();
            resource.pullRequestId = "1";
            resource.pullRequestTitle = "demo title";
            dynamic repository = new ExpandoObject();
            repository.Id = "1";
            repository.Name = "demo repo";
            requestBody.resource = resource;
            requestBody.resource.repository = repository;

            var mockCosmosDbService = new Mock<ICosmosDbRepository<DeploymentTemplate>>();
            
            var expected = new CMPGitRepository { Id = requestBody.resource.repository.Id, Name = requestBody.resource.repository.Name };
            
                 
            var mockRequest = TestingHelper.CreateMockRequest(requestBody);
            var mockLoggerFunction = TestingHelper.GetLogger<RepoEventsFunction>();

            mockService.Setup(x => x.GetRepositoryAsync()).ReturnsAsync(expected);
            

            var functionUnderTest = new RepoEventsFunction(mockLoggerFunction.Object, OptionsConfig, mockService.Object, mockCosmosDbService.Object);

            var result = functionUnderTest.Run(mockRequest.Object).GetAwaiter().GetResult();            

            Assert.IsTrue(result is OkResult);            
        }

    }
}
