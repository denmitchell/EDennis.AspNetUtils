using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EDennis.AspNetUtils.Core.Services.Wasm
{
    /// <summary>
    /// https://stackoverflow.com/a/66063573
    /// </summary>
    public class FakeAuthStateProvider : AuthenticationStateProvider, IAccessTokenProvider
    {

        private readonly IServiceProvider _provider;

        public virtual string InfoEndpoint { get; } = "Me/Info";

        public FakeAuthStateProvider(IServiceProvider serviceProvider) {
            _provider = serviceProvider;    
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {

            var httpClient = _provider.GetRequiredService<HttpClient>();
            var appUser = await httpClient.GetFromJsonAsync<AppUser>(InfoEndpoint);

            var user = CreateFakeClaimsPrincipal(appUser);

            return await Task.FromResult(new AuthenticationState(user));
        }

        public async ValueTask<AccessTokenResult> RequestAccessToken()
        {
            return await Task.Run(()=>new AccessTokenResult(AccessTokenResultStatus.Success, new AccessToken() { Expires = DateTime.Now + new TimeSpan(365, 0, 0, 0) }, "", 
                new InteractiveRequestOptions { Interaction = InteractionType.GetToken, ReturnUrl = "~/"}));
        }

        public async ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            return await Task.Run(() => new AccessTokenResult(AccessTokenResultStatus.Success, new AccessToken() { Expires = DateTime.Now + new TimeSpan(365, 0, 0, 0) }, "",
                new InteractiveRequestOptions { Interaction = InteractionType.GetToken, ReturnUrl = "~/" }));
        }

        /// <summary>
        /// Updates the claims principal to include role claims from <see cref="AppUser"/>
        /// retrieved via the <see cref="InfoEndpoint"/>
        /// </summary>
        /// <param name="principal">The authenticated user</param>
        /// <param name="appUser">Holds claims to be added to that user</param>
        private static ClaimsPrincipal CreateFakeClaimsPrincipal(AppUser appUser)
        {

            Claim[] claims = appUser.Role.Split(',').SelectMany(r => new Claim[]
            {
                new Claim("role", r),
                new Claim(ClaimTypes.Role, r)
            }).
            Union(new Claim[] {
                new Claim("name", appUser.UserName),
                new Claim(ClaimTypes.Name, appUser.UserName),
            }).ToArray();


            return new ClaimsPrincipal(new ClaimsIdentity(claims, "Fake authentication type"));

        }


    }
}
