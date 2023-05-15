namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Client.Services
{
    public class AppRoleApiClientService : ApiClientService<AppRole>
    {
        public AppRoleApiClientService(HttpClient client) : base(client)
        {
        }
    }
}
