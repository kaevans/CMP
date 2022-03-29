using CMP.Functions.Models;
using CMP.Functions.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CMP.Functions.Startup))]

namespace CMP.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<GitRepositoryOptions>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(GitRepositoryOptions.SectionName).Bind(settings);

            });

            builder.Services.AddSingleton<IGitRepositoryService, AzureDevOpsGitRepositoryService>();

        }
    }
}