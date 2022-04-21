using CMP.Core.Interfaces;
using CMP.Core.Models;

using CMP.Infrastructure.Data;
using CMP.Infrastructure.Git;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(CMP.Functions.Startup))]

namespace CMP.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var context = builder.GetContext();
            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false)                
                .AddUserSecrets("47c1dc54-67e8-4418-b998-2c3f1462362c")
                .AddEnvironmentVariables();        
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<GitRepoOptions>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(GitRepoOptions.SectionName).Bind(settings);

            });
            builder.Services.AddSingleton<IGitRepoRepository, ADORepoRepository>();

            builder.Services.AddSingleton<IGitRepoItemRepositoryFactory<DeploymentTemplate>, ADODeploymentTemplateRepositoryFactory>();

            builder.Services.AddOptions<CosmosDbOptions>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(CosmosDbOptions.SectionName).Bind(settings);
            });            
            builder.Services.AddSingleton<ICosmosDbRepository<DeploymentTemplate>, DeploymentTemplateRepository>();
        }

    }
}