using Microsoft.AspNetCore.Mvc;

namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppRoleController : CrudController<AppRole>
    {
        public AppRoleController(ICrudService<AppRole> crudService, ILoggerFactory loggerFactory) : base(crudService, loggerFactory)
        {
        }
    }
}
