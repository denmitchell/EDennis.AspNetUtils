using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Used with <see cref="Extensions.AddCrudServices{TContext}(WebApplicationBuilder, string)"/>
    /// to streamline the configuration in Program.cs.
    /// </summary>
    /// <typeparam name="TContext">The DbContext type</typeparam>
    public class CrudServiceConfigurationBuilder<TContext>
        where TContext : DbContext
    {
        private readonly WebApplicationBuilder _builder;

        /// <summary>
        /// Constructs a new instance of <see cref="CrudServiceConfigurationBuilder{TContext}"/>
        /// for adding crud services
        /// </summary>
        /// <param name="builder"></param>
        public CrudServiceConfigurationBuilder(WebApplicationBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Adds an individual <see cref="EntityFrameworkService{TContext, TEntity}"/>.  Note:
        /// This method configures DI for the following services:
        /// <list type="bullet">
        /// <item><see cref="CrudServiceDependencies{TContext, TEntity}"/></item>
        /// <item><see cref="EntityFrameworkService{TContext, TEntity}"/></item>
        /// <item><see cref="CountCache{TEntity}"/></item>
        /// </list>
        /// </summary>
        /// <typeparam name="TService">The CrudService Type</typeparam>
        /// <typeparam name="TEntity">The Entity/Model Type</typeparam>
        /// <returns></returns>
        public CrudServiceConfigurationBuilder<TContext> AddCrudService<TService,TEntity>() 
            where TEntity : class
            where TService : EntityFrameworkService<TContext, TEntity>
        {
            _builder.Services.TryAddScoped<CrudServiceDependencies<TContext, TEntity>>();
            _builder.Services.TryAddScoped<TService>();
            _builder.Services.TryAddScoped<CountCache<TEntity>>();
            return this;
        }
    }

}
