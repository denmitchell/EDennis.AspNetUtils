using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace EDennis.AspNetUtils
{
    public static class MsalWasmExtensions
    {
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
