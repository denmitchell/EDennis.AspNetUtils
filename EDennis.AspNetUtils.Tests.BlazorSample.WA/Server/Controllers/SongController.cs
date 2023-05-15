using EDennis.AspNetUtils.Tests.BlazorSample.Shared.Models;

namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Server.Controllers
{
    public class SongController : CrudController<Song>
    {
        public SongController(ICrudService<Song> crudService, ILoggerFactory loggerFactory) : base(crudService, loggerFactory)
        {
        }
    }
}
