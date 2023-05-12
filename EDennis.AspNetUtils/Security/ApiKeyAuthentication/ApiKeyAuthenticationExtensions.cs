using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EDennis.AspNetUtils
{
    public static class ApiKeyAuthenticationExtensions
    {
        /// <summary>
        /// Adds ApiKeyAuthentication to an API.  This is added to a project that exposes the
        /// API via a controller.
        /// </summary>
        /// <param name="services">A reference to the IServiceCollection instance</param>
        /// <param name="config">A reference to IConfiguration instance</param>
        /// <param name="configKey">The configuration key for the section (default to Security:ApiKey)</param>
        /// <param name="additionalConfigure">an optional action to configure authentication options further</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddApiKeyAuthentication(this IServiceCollection services,
            IConfiguration config, string configKey = null, Action<AuthenticationOptions> additionalConfigure = null
            )
        {
            configKey ??= ApiKeyAuthenticationOptions.DefaultConfigKey;
            if (!services.Any(s => s.ServiceType == typeof(IOptionsMonitor<ApiKeyAuthenticationOptions>)))
            {
                services.BindAndConfigure(config, configKey, out ApiKeyAuthenticationOptions _);
            }

            AuthenticationBuilder builder = null;
            builder = services.AddAuthentication(
                    options =>
                    {
                        options.DefaultScheme = AuthenticationSchemeConstants.ApiKeyAuthenticationScheme;
                        additionalConfigure?.Invoke(options);
                    })
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                    AuthenticationSchemeConstants.ApiKeyAuthenticationScheme, options => { });

            return builder;
        }

    }
}
