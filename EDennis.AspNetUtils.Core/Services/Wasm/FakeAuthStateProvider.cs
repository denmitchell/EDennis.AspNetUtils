using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
        private readonly SecurityOptions _securityOptions;

        public virtual string InfoEndpoint { get; } = "Me/Info";

        public FakeAuthStateProvider(IServiceProvider serviceProvider, IOptionsMonitor<SecurityOptions> securityOptions) {
            _provider = serviceProvider;
            _securityOptions = securityOptions.CurrentValue;
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
            return await Task.Run(()=>new AccessTokenResult(AccessTokenResultStatus.Success, 
                new AccessToken() { Expires = DateTime.Now + new TimeSpan(365, 0, 0, 0), 
                    GrantedScopes= new string[] { _securityOptions.ApiAccessClaim }, 
                    Value = GenerateToken()}, "", 
                new InteractiveRequestOptions { Interaction = InteractionType.GetToken, ReturnUrl = "~/"}));
        }

        public async ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            return await Task.Run(() => new AccessTokenResult(AccessTokenResultStatus.Success, 
                new AccessToken() { Expires = DateTime.Now + new TimeSpan(365, 0, 0, 0), 
                    GrantedScopes = new string[] { _securityOptions.ApiAccessClaim },
                    Value = GenerateToken()
                }, "",
                new InteractiveRequestOptions { Interaction = InteractionType.GetToken, ReturnUrl = "~/" }));
        }

        /// <summary>
        /// Updates the claims principal to include role claims from <see cref="AppUser"/>
        /// retrieved via the <see cref="InfoEndpoint"/>
        /// </summary>
        /// <param name="principal">The authenticated user</param>
        /// <param name="appUser">Holds claims to be added to that user</param>
        private ClaimsPrincipal CreateFakeClaimsPrincipal(AppUser appUser)
        {

            Claim[] claims = appUser.Role.Split(',').SelectMany(r => new Claim[]
            {
                new Claim("role", r),
                new Claim(ClaimTypes.Role, r)
            }).
            Union(new Claim[] {
                new Claim("name", appUser.UserName),
                new Claim(ClaimTypes.Name, appUser.UserName),
                new Claim("scope", appUser.Scope)//_securityOptions.ApiAccessClaim)
            }).ToArray();


            return new ClaimsPrincipal(new ClaimsIdentity(claims, "Fake authentication type"));

        }


        public string GenerateToken()
        {
            var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = "http://mysite.com";
            var myAudience = "http://myaudience.com";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "xxx"),
                }),
                Claims = new Dictionary<string,object> { {"scp",_securityOptions.ApiAccessClaim } },
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
