using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Used with <see cref="ConfigurationExtensions.AddApiClientServices(WebApplicationBuilder, string)"/>
    /// to streamline the configuration in Program.cs.
    /// </summary>
    /// <typeparam name="TContext">The DbContext type</typeparam>
    public class ApiClientServiceBuilder
    {
        private readonly WebApplicationBuilder _builder;
        private readonly ApiClientSettingsDictionary _apis;
        private readonly bool _addApiKeyMessageHandler;
        /// <summary>
        /// Constructs a new instance of <see cref="ApiClientServiceBuilder{TContext}"/>
        /// for adding crud services
        /// </summary>
        /// <param name="builder"></param>
        public ApiClientServiceBuilder(WebApplicationBuilder builder, ApiClientSettingsDictionary apis, bool addApiKeyMessageHandler)
        {
            _builder = builder;
            _apis = apis;
            _addApiKeyMessageHandler = addApiKeyMessageHandler;
        }

        /// <summary>
        /// Adds an individual <see cref="ApiClientService{TEntity}"/>.  Note:
        /// This method configures DI for the following services:
        /// <list type="bullet">
        /// <item><see cref="ApiClientService{TEntity}"/></item>
        /// </list>
        /// </summary>
        /// <typeparam name="TService">The CrudService Type</typeparam>
        /// <typeparam name="TEntity">The Entity/Model Type</typeparam>
        /// <returns></returns>
        public ApiClientServiceBuilder AddApiClientService<TEntity>()
            where TEntity : class
        {
            if (!_apis.TryGetValue(typeof(TEntity).Name, out ApiClientSettings api))
                throw new Exception($"Configuration for ApiClient {typeof(TEntity).Name} not set");


            IHttpClientBuilder httpClientBuilder = _builder.Services.AddHttpClient<ApiClientService<TEntity>>(configure =>
            {
                configure.BaseAddress = new Uri(api.BaseAddress);
            });

            if (_addApiKeyMessageHandler)
                httpClientBuilder.AddHttpMessageHandler<ApiKeyMessageHandler>();
            _builder.Services.TryAddScoped<ICrudService<TEntity>, ApiClientService<TEntity>>();

            return this;
        }
    }

}
