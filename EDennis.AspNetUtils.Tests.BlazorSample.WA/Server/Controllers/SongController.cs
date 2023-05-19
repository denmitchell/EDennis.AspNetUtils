using EDennis.AspNetUtils.Tests.BlazorSample.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class SongController : CrudController<Song>
    {
        public SongController(ICrudService<Song> crudService, ILoggerFactory loggerFactory) : base(crudService, loggerFactory)
        {
        }
    }
}
