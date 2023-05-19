using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class AppUserController : CrudController<AppUser>
    {
        public AppUserController(ICrudService<AppUser> crudService, ILoggerFactory loggerFactory) 
            : base(crudService, loggerFactory)
        {
        }
    }
}
