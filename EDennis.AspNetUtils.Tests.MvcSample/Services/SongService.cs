using EDennis.AspNetUtils.Tests.MvcSample;

namespace EDennis.AspNetUtils.Tests.MvcSample.Services
{
    public class SongService : CrudService<HitsContext, Song>
    {
        public SongService(CrudServiceDependencies<HitsContext, Song> deps) : base(deps) { }
    }
}
