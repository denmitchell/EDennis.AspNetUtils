using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EDennis.AspNetUtils
{
    public static class ApiClientServiceExtensions
    {
        /// <summary>
        /// Configures DI for a set of named HttpClients.  The name of the HttpClient is
        /// the key for the client in configuration.  These ApiClients use ApiKey security
        /// via <see cref="ApiKeyMessageHandler"/>
        /// </summary>
        /// <param name="services">Reference to the IServiceCollection instance</param>
        /// <param name="config">Reference to the IConfiguration instance</param>
        /// <param name="configKey">Parent key for each API settings section</param>
        /// <param name="securityConfigKey">Configuration key for a token service used with child APIs</param>
        /// <returns></returns>
        public static ApiClientServiceBuilder AddApiClientServices(this WebApplicationBuilder builder,
            bool addApiKeyMessageHandler = true, string configKey = "Apis", string securityConfigKey = "Security")
        {
            //get the settings from configuration and also setup
            //DI to inject IOptionsMonitor<ApiClientSettingsDictionary>
            builder.Services.BindAndConfigure(builder.Configuration, configKey, out ApiClientSettingsDictionary apis);


            if (!builder.Services.Any(s => s.ServiceType == typeof(IOptionsMonitor<SecurityOptions>)))
            {
                builder.Services.BindAndConfigure(builder.Configuration, securityConfigKey, out SecurityOptions _);
            }

            if (addApiKeyMessageHandler)
                builder.Services.AddTransient<ApiKeyMessageHandler>();

            if (!builder.Services.Any(s => s.ServiceType == typeof(IHttpContextAccessor)))
                builder.Services.AddHttpContextAccessor();

            return new ApiClientServiceBuilder(builder, apis, addApiKeyMessageHandler);
        }





    }
}
