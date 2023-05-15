using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Security.Claims;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Implementation of <see cref="AccountClaimsPrincipalFactory{MsalUserAccount}"/> that retrieves
    /// role claims from a 
    /// retrieves
    /// </summary>
    public class MsalAccountClaimsPrincipalFactory : AccountClaimsPrincipalFactory<MsalUserAccount>
    {
        private readonly IServiceProvider _provider;

        public virtual string InfoEndpoint { get; } = "Me/Info";

        public MsalAccountClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor,
            IServiceProvider serviceProvider) : base(accessor)
        {
            _provider = serviceProvider;
        }

        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(MsalUserAccount account, RemoteAuthenticationUserOptions options)
        {
            var initialUser = await base.CreateUserAsync(account, options);

            if (initialUser.Identity != null && initialUser.Identity.IsAuthenticated)
            {
                var httpClient = _provider.GetRequiredService<HttpClient>();
                var appUser = await httpClient.GetFromJsonAsync<AppUser>(InfoEndpoint);
                UpdateClaimsPrincipal(initialUser, appUser);
            }

            return initialUser;
        }

        private static void UpdateClaimsPrincipal(ClaimsPrincipal principal, AppUser appUser)
        {

            Claim[] claims = appUser.Role.Split(',').SelectMany(r => new Claim[]
            {
                new Claim("role", r),
                new Claim(ClaimTypes.Role, r)
            }).ToArray();

            (principal.Identity as ClaimsIdentity).AddClaims(claims);

        }

    }
}
