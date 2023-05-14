using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace EDennis.AspNetUtils
{
    public static class ApiClientServiceWasmExtensions
    {
        /// <summary>
        /// Configures an ApiClientService for use with 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static WebAssemblyHostBuilder AddApiClientService(this WebAssemblyHostBuilder builder,
            string serviceName)
        {

            builder.Services.AddHttpClient(serviceName, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient(serviceName));

            return builder;
        }





    }
}
