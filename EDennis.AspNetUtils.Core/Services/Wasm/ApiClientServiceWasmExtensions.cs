using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Extension methods to facilitate configuration of API Clients
    /// </summary>
    public static class ApiClientServiceWasmExtensions
    {
        /// <summary>
        /// Configures an ApiClientService for use with the WASM Server API
        /// </summary>
        /// <param name="builder">builder from Program.cs</param>
        /// <param name="serviceName">The name of the service</param>
        /// <returns></returns>
        public static WebAssemblyApiClientServiceBuilder AddApiClientServices(this WebAssemblyHostBuilder builder,
            string serviceName = null)
        {
            serviceName ??= "ServerApi";

            builder.Services.AddHttpClient(serviceName, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient(serviceName));

            return new WebAssemblyApiClientServiceBuilder(builder);
        }


    }
}
