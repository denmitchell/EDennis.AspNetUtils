namespace EDennis.AspNetUtils.Tests.BlazorSample.Services
{
    public class SongService : EntityFrameworkService<HitsContext, Song>
    {
        public SongService(CrudServiceDependencies<HitsContext, Song> deps) : base(deps) { }
    }
}
