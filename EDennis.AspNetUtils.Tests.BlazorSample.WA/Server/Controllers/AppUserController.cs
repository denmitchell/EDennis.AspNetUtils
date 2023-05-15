namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Server.Controllers
{
    public class AppUserController : CrudController<AppUser>
    {
        public AppUserController(ICrudService<AppUser> crudService, ILoggerFactory loggerFactory) 
            : base(crudService, loggerFactory)
        {
        }
    }
}
