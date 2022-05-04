using CMP.Core.Models;
using CMP.Infrastructure.Cloud.Azure.ResourceManagement;
using CMP.Infrastructure.Data;
using CMP.Infrastructure.Git;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using NuGet.Configuration;
using CMP.Infrastructure.Search;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CMP.Web.Models;
using CMP.Web.Services;

namespace CMP.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));

            services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy.
                options.FallbackPolicy = options.DefaultPolicy;
            });

            services.AddApplicationInsightsTelemetry(Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

            services.AddOptions<GitRepoOptions>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(GitRepoOptions.SectionName).Bind(settings);
            });
            services.AddSingleton<IGitRepoRepository, ADORepoRepository>();

            services.AddOptions<CosmosDbOptions>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(CosmosDbOptions.SectionName).Bind(settings);
            });
            services.AddSingleton<ICosmosDbRepository<DeploymentTemplate>, DeploymentTemplateRepository>();

            /*
            services.AddOptions<ResourceManagementOptions>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(ResourceManagementOptions.SectionName).Bind(settings);
            });                  
            services.AddHttpClient<IResourceManagementService, ResourceManagementService>();
            */

            services.AddOptions<AzureSearchOptions>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(AzureSearchOptions.SectionName).Bind(settings);
            });
            services.AddSingleton<ISearchService<DeploymentTemplateSearchResult>, DeploymentTemplateSearchService>();

            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddMicrosoftIdentityUI();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
