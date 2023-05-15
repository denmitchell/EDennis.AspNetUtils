using EDennis.AspNetUtils.Tests.BlazorSample.Shared.Models;

namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Client.Services
{
    public class ArtistApiClientService : ApiClientService<Artist>
    {
        public ArtistApiClientService(HttpClient client) : base(client)
        {
        }
    }
}
