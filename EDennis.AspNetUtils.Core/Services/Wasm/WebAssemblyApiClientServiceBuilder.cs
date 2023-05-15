using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EDennis.AspNetUtils
{
    public class WebAssemblyApiClientServiceBuilder
    {
        private readonly WebAssemblyHostBuilder _builder;
        public WebAssemblyApiClientServiceBuilder(WebAssemblyHostBuilder builder)
        {
            _builder = builder;
        }

        public WebAssemblyApiClientServiceBuilder AddApiClientService<TEntity>()
            where TEntity : class
        {
            _builder.Services.TryAddScoped<ICrudService<TEntity>, ApiClientService<TEntity>>();
            return this;
        }

    }
}
