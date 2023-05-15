using EDennis.AspNetUtils.Tests.BlazorSample.Shared.Models;

namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Server.Controllers
{
    public class ArtistController : CrudController<Artist>
    {
        public ArtistController(ICrudService<Artist> crudService, ILoggerFactory loggerFactory) : base(crudService, loggerFactory)
        {
        }
    }
}
