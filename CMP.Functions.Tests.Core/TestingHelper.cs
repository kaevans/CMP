using CMP.Infrastructure.Data;
using CMP.Infrastructure.Git;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CMP.Functions.Tests.Core
{
    public static class TestingHelper
    {
        public static Mock<ILogger<T>> GetLogger<T>()
        {
            var logger = new Mock<ILogger<T>>();

            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var logLevel = (LogLevel)invocation.Arguments[0]; // The first two will always be whatever is specified in the setup above
                    var eventId = (EventId)invocation.Arguments[1];  // so I'm not sure you would ever want to actually use them
                    var state = invocation.Arguments[2];
                    var exception = (Exception)invocation.Arguments[3];
                    var formatter = invocation.Arguments[4];

                    var invokeMethod = formatter.GetType().GetMethod("Invoke");
                    var logMessage = (string)invokeMethod?.Invoke(formatter, new[] { state, exception });

                    Trace.WriteLine($"{logLevel} - {logMessage}");
                }));

            return logger;
        }

        public static Mock<HttpRequest> CreateMockRequestWithJson(string json)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            sw.Write(json);
            sw.Flush();

            ms.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);

            return mockRequest;
        }

        public static Mock<HttpRequest> CreateMockRequest(object body)
        {
            var json = JsonConvert.SerializeObject(body);
            return CreateMockRequestWithJson(json);
        }


        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            //TODO: Validte if the usersecrets.json GUID changes
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets("426bd3ab-8222-415e-977d-6529dafc9f78")
                .AddEnvironmentVariables()
                .Build();
        }

        public static GitRepoOptions GetApplicationConfiguration(string outputPath)
        {
            var configuration = new GitRepoOptions();

            var iConfig = GetIConfigurationRoot(outputPath);

            iConfig.GetSection(GitRepoOptions.SectionName).Bind(configuration);

            return configuration;
        }

        public static CosmosDbOptions GetCosmosDBConfiguration(string outputPath)
        {
            var configuration = new CosmosDbOptions();

            var iConfig = GetIConfigurationRoot(outputPath);

            iConfig.GetSection(CosmosDbOptions.SectionName).Bind(configuration);

            return configuration;
        }
    }
}