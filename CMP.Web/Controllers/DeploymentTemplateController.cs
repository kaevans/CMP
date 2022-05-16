using CMP.Core.Models;
using CMP.Infrastructure.CosmosDb;
using CMP.Infrastructure.Git;
using CMP.Infrastructure.Search;
using CMP.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CMP.Web.Controllers
{
    [Authorize]
    public class DeploymentTemplateController : Controller
    {
        private readonly ILogger<DeploymentTemplateController> _logger;
        private readonly ICosmosDbRepository<DeploymentTemplate> _cosmosDbRepositoryService;
        private readonly ISearchService<DeploymentTemplateSearchResult> _searchService;
        

        public DeploymentTemplateController(
            ILogger<DeploymentTemplateController> logger,
            ICosmosDbRepository<DeploymentTemplate> cosmosDbRepositoryService,
            ISearchService<DeploymentTemplateSearchResult> searchService)            
        {
            _logger = logger;
            _cosmosDbRepositoryService = cosmosDbRepositoryService;
            _searchService = searchService;
        }


        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            var featuredItems = await _cosmosDbRepositoryService.GetItemsAsync();

            return View(featuredItems);
        }

        [HttpPost]
        [ActionName("Search")]
        public async Task<IActionResult> Search(SearchData model)
        {
            var results = await _searchService.SearchAsync(model);                                    
            
            return View(results);
        }

        [ActionName("View")]
        public async Task<IActionResult> GetById(string id)
        {
            return View(await _cosmosDbRepositoryService.GetByIdAsync(id));
        }
    }
}
