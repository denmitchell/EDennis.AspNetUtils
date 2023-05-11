namespace EDennis.AspNetUtils.Tests.MvcSample.Services
{
    public class ArtistService : EntityFrameworkService<HitsContext, Artist>
    {
        public ArtistService(CrudServiceDependencies<HitsContext, Artist> deps): base(deps) { }
    }
}
