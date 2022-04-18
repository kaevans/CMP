using CMP.Core.Models;
using CMP.Infrastructure.Data;
using CMP.Infrastructure.Git;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mvcBuilder = builder.Services.AddControllersWithViews();
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    mvcBuilder.AddRazorRuntimeCompilation();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
