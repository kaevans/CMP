using CMP.Core.Models.Azure;
using CMP.Infrastructure.Cloud;
using CMP.Infrastructure.Cloud.Azure.ResourceManagement;
using CMP.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace CMP.Web.Controllers
{
    [AuthorizeForScopes(Scopes = new[] { "https://management.core.windows.net/user_impersonation" })]
    public class PipelineDeploymentController : Controller
    {
        private readonly ILogger<PipelineDeploymentController> _logger;
        private readonly ResourceManagementOptions _options;
        private readonly IResourceManagementService _azureService;        

        public PipelineDeploymentController(
            ILogger<PipelineDeploymentController> logger,
            IOptions<ResourceManagementOptions> options,
            IResourceManagementService azureService
            )
        {
            _logger = logger;
            _options = options.Value;
            _azureService = azureService;            
        }

        // GET: SubscriptionController        
        public async Task<ActionResult> Index()
        {
            var ret = new SubscriptionModel();

            var subscriptionList = await _azureService.GetSubscriptionsAsync();

            var resourceGroupList = (List<ResourceGroup>)await _azureService.GetResourceGroupsAsync(subscriptionList.First().SubscriptionId);

            ret.Subscriptions = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(subscriptionList, "SubscriptionId", "DisplayName", subscriptionList.First().SubscriptionId);
            ret.ResourceGroups = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(resourceGroupList, "Id", "Name", resourceGroupList.First().Id);

            return View(ret);
        }

        [HttpPost]        
        public async Task<JsonResult> setDropDrownList(string type, string value)
        {
            var model = new SubscriptionModel();

            var resourceGroupList = await _azureService.GetResourceGroupsAsync(value);
            model.ResourceGroups = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(resourceGroupList, "Id", "Name");
                        
            return Json(model);
        }

        [HttpPost]        
        public async Task<ActionResult> Index(SubscriptionModel model)
        {
            var subscriptionList = await _azureService.GetSubscriptionsAsync();

            if (subscriptionList.Any())
            {
                model.Subscriptions = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(subscriptionList, "SubscriptionId", "DisplayName", model.SubscriptionId);

                var resourceGroupList = await _azureService.GetResourceGroupsAsync(model.SubscriptionId);
                if (resourceGroupList.Any())
                {
                    //Add the resource groups for the first subscription
                    model.ResourceGroups = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(resourceGroupList, "Id", "Name", model.ResourceGroupId);
                }
                else
                {
                    model.ResourceGroups = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(resourceGroupList, "Id", "DisplayName");
                }
            }
            else
            {
                //No subscriptions returned, either none exist or the user doesn't have permission
                model.Subscriptions = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(new List<Subscription>(), "SubscriptionId", "DisplayName");
                model.ResourceGroups = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(new List<ResourceGroup>(), "Id", "DisplayName");
            }
            
            return View(model);
        }

    }
}
