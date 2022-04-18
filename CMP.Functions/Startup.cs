﻿using CMP.Core.Interfaces;
using CMP.Core.Models;

using CMP.Infrastructure.Data;
using CMP.Infrastructure.Git;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(CMP.Functions.Startup))]

namespace CMP.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<GitRepoOptions>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(GitRepoOptions.SectionName).Bind(settings);

            });
            builder.Services.AddSingleton<IGitRepoRepository<DeploymentTemplate>, AzureDevOpsGitRepoRepository>();
            
            builder.Services.AddOptions<CosmosDbOptions>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(CosmosDbOptions.SectionName).Bind(settings);
            });            
            builder.Services.AddSingleton<ICosmosDbRepository<DeploymentTemplate>, DeploymentTemplateRepository>();
        }

    }
}