using EDennis.AspNetUtils.Tests.BlazorSample.Shared.Models;

namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Client.Services
{
    public class SongApiClientService : ApiClientService<Song>
    {
        public SongApiClientService(HttpClient client) : base(client)
        {
        }
    }
}
