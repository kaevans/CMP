using CMP.Core.Models;
using CMP.Infrastructure.Data;
using CMP.Infrastructure.Git;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CMP.Web.Controllers
{
    public class DeploymentTemplateController : Controller
    {
        private readonly ILogger<DeploymentTemplateController> _logger;
        private readonly GitRepoOptions _options;
        private readonly IGitRepoRepository _gitRepositoryService;
        private readonly ICosmosDbRepository<DeploymentTemplate> _cosmosDbRepositoryService;

        public DeploymentTemplateController(ILogger<DeploymentTemplateController> logger,
            IOptions<GitRepoOptions> options,
            IGitRepoRepository gitRepositoryService,
            ICosmosDbRepository<DeploymentTemplate> cosmosDbRepositoryService
            )
        {
            _logger = logger;
            _options = options.Value;
            _gitRepositoryService = gitRepositoryService;
            _cosmosDbRepositoryService = cosmosDbRepositoryService;
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _cosmosDbRepositoryService.GetItemsAsync());
        }

        [ActionName("View")]
        public async Task<IActionResult> GetById(string id)
        {
            return View(await _cosmosDbRepositoryService.GetByIdAsync(id));
        }
    }
}
