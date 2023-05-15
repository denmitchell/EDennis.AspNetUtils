using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            }).AddAccountClaimsPrincipalFactory<RemoteAuthenticationState,
                    MsalUserAccount, MsalAccountClaimsPrincipalFactory>();
        }
    }
}
