using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class AppRoleController : CrudController<AppRole>
    {
        public AppRoleController(ICrudService<AppRole> crudService, ILoggerFactory loggerFactory) : base(crudService, loggerFactory)
        {
        }
    }
}
