using EDennis.AspNetUtils.Tests.BlazorSample.Shared.Models;

namespace EDennis.AspNetUtils.Tests.BlazorSample.Services
{
    public class ArtistService : EntityFrameworkService<HitsContext, Artist>
    {
        public ArtistService(EntityFrameworkServiceDependencies<HitsContext, Artist> deps): base(deps) { }


        /// <summary>
        /// Delete all songs belonging to the artist before deleting the artist
        /// </summary>
        /// <param name="existing"></param>
        public override void BeforeDelete(Artist existing)
        {
            var songs = DbContext.Set<Song>()
                .Where(u => u.ArtistId == existing.Id)
                .ToList();

            DbContext.RemoveRange(songs);
            DbContext.SaveChanges();
        }

    }
}
