using EDennis.AspNetUtils.Tests.BlazorSample;
using Microsoft.EntityFrameworkCore;

namespace EDennis.AspNetUtils.Tests.BlazorSample.Services
{
    public class ArtistService : CrudService<HitsContext, Artist>
    {
        public ArtistService(CrudServiceDependencies<HitsContext, Artist> deps): base(deps) { }


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
