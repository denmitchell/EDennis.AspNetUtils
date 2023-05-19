using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Security.Claims;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Implementation of <see cref="AccountClaimsPrincipalFactory{MsalUserAccount}"/> 
    /// that retrieves role claims from the logged on user.  Multiple roles may be
    /// transmitted via a comma-delimited string
    /// </summary>
    public class MsalAccountClaimsPrincipalFactory : AccountClaimsPrincipalFactory<MsalUserAccount>
    {
        private readonly IServiceProvider _provider;

        public virtual string InfoEndpoint { get; } = "Me/Info";

        /// <summary>
        /// Instantiates a new <see cref="MsalAccountClaimsPrincipalFactory"/> with
        /// the provided services and configurations.
        /// </summary>
        /// <param name="accessor">Reference to the token provider</param>
        /// <param name="serviceProvider">Reference to the service provider (for IHttpClientFactory)</param>
        /// <param name="config">Reference to IConfiguration instance</param>
        /// <param name="infoEndpointConfigKey">when not null, the relative URL for <see cref="InfoEndpoint"/> is taken from configuration</param>
        public MsalAccountClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor,
            IServiceProvider serviceProvider, IConfiguration config, string infoEndpointConfigKey = null) : base(accessor)
        {
            if (infoEndpointConfigKey != null) { 
                InfoEndpoint = config[infoEndpointConfigKey];
            }
            _provider = serviceProvider;
        }

        /// <summary>
        /// Creates a new user based upon information retrieved via the <see cref="InfoEndpoint"/>
        /// </summary>
        /// <param name="account"><see cref="RemoteUserAccount"/> implementation</param>
        /// <param name="options">Remote authentication options</param>
        /// <returns></returns>
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

        /// <summary>
        /// Updates the claims principal to include role claims from <see cref="AppUser"/>
        /// retrieved via the <see cref="InfoEndpoint"/>
        /// </summary>
        /// <param name="principal">The authenticated user</param>
        /// <param name="appUser">Holds claims to be added to that user</param>
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
