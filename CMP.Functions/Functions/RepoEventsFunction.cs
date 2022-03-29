using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using CMP.Functions.Models;
using CMP.Functions.Services;
using System.Runtime.CompilerServices;

namespace CMP.Functions
{
    public class RepoEventsFunction
    {
        private readonly ILogger<RepoEventsFunction> _logger;
        private readonly GitRepositoryOptions _options;
        private readonly IGitRepositoryService _gitRepositoryService;

        public RepoEventsFunction(ILogger<RepoEventsFunction> logger, 
            IOptions<GitRepositoryOptions> options,
            IGitRepositoryService gitRepositoryService)
        {
            _logger = logger;
            _options = options.Value;
            _gitRepositoryService = gitRepositoryService;
        }


        [FunctionName("RepoEvents")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            _logger.LogInformation(_options.PersonalAccessToken);
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
