using EDennis.AspNetUtils.Tests.MvcSample;

namespace EDennis.AspNetUtils.Tests.MvcSample.Services
{
    public class ArtistService : CrudService<HitsContext, Artist>
    {
        public ArtistService(CrudServiceDependencies<HitsContext, Artist> deps): base(deps) { }
    }
}
