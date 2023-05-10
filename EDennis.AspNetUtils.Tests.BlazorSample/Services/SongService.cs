using EDennis.AspNetUtils.Tests.BlazorSample;

namespace EDennis.AspNetUtils.Tests.BlazorSample.Services
{
    public class SongService : CrudService<HitsContext, Song>
    {
        public SongService(CrudServiceDependencies<HitsContext, Song> deps) : base(deps) { }
    }
}
