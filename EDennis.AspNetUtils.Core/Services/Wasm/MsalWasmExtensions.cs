using EDennis.AspNetUtils.Core.Services.Wasm;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Authentication.WebAssembly.Msal.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Defines <see cref="WebAssemblyHostBuilder"/> extensions methods
    /// </summary>
    public static class MsalWasmExtensions
    {
        /// <summary>
        /// Configures a webassembly client project to use Microsoft Authentication Library and
        /// SimpleAuthorization.  Note: this configures the application to use 
        /// <see cref="MsalAccountClaimsPrincipalFactory"/> and <see cref="MsalUserAccount"/>
        /// </summary>
        /// <param name="builder">A reference to the Program.cs builder</param>
        /// <param name="azureAdConfigKey">Configuration key for AzureAd</param>
        /// <param name="serverApiConfigSection">Configuration key for ServerApi</param>
        /// <param name="scopeConfigSection">Configuration key for Scopes</param>
        /// <returns></returns>
        public static IRemoteAuthenticationBuilder<RemoteAuthenticationState,MsalUserAccount>
            AddMsalAuthentication(this WebAssemblyHostBuilder builder, string azureAdConfigKey = "AzureAd",
            string serverApiConfigSection = "ServerApi", string scopeConfigSection = "Scopes")
        {

            return builder.Services.AddMsalAuthentication<RemoteAuthenticationState, MsalUserAccount>(options =>
            {
                builder.Configuration.Bind(azureAdConfigKey, options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add(
                    builder.Configuration.GetSection(serverApiConfigSection)[scopeConfigSection]);
                options.ProviderOptions.LoginMode = "redirect";
            }).AddAccountClaimsPrincipalFactory<RemoteAuthenticationState,
                    MsalUserAccount, MsalAccountClaimsPrincipalFactory>();
        }

        public static void SetupFakeAuth(this WebAssemblyHostBuilder builder)
        {
            //https://github.com/dotnet/aspnetcore/blob/c925f99cddac0df90ed0bc4a07ecda6b054a0b02/src/Components/WebAssembly/WebAssembly.Authentication/src/WebAssemblyAuthenticationServiceCollectionExtensions.cs#L28

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.TryAddScoped<AuthenticationStateProvider, FakeAuthStateProvider>();


            builder.Services.TryAddTransient<BaseAddressAuthorizationMessageHandler>();
            builder.Services.TryAddTransient<AuthorizationMessageHandler>();

            builder.Services.TryAddScoped(sp =>
            {
                return (IAccessTokenProvider)sp.GetRequiredService<AuthenticationStateProvider>();
            });


            builder.Services.AddRemoteAuthentication<RemoteAuthenticationState, MsalUserAccount, MsalProviderOptions>();

            builder.Services.TryAddScoped<IAccessTokenProviderAccessor, FakeAccessTokenProviderAccessor>();

        }

    }
}
