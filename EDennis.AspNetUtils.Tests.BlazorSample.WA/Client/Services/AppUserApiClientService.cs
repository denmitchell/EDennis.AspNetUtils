namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Client.Services
{
    public class AppUserApiClientService : ApiClientService<AppUser>
    {
        public AppUserApiClientService(HttpClient client) : base(client)
        {
        }
    }
}
