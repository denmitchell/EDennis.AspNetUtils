using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Used with <see cref="EntityFrameworkServiceExtensions.AddEntityFrameworkServices{TContext}(WebApplicationBuilder, string)"/>
    /// to streamline the configuration in Program.cs.
    /// </summary>
    /// <typeparam name="TContext">The DbContext type</typeparam>
    public class EntityFrameworkServiceBuilder<TContext>
        where TContext : DbContext
    {
        private readonly WebApplicationBuilder _builder;

        /// <summary>
        /// Constructs a new instance of <see cref="EntityFrameworkServiceBuilder{TContext}"/>
        /// for adding crud services
        /// </summary>
        /// <param name="builder"></param>
        public EntityFrameworkServiceBuilder(WebApplicationBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Adds an individual <see cref="EntityFrameworkService{TContext, TEntity}"/>.  Note:
        /// This method configures DI for the following services:
        /// <list type="bullet">
        /// <item><see cref="EntityFrameworkServiceDependencies{TContext, TEntity}"/></item>
        /// <item><see cref="EntityFrameworkService{TContext, TEntity}"/></item>
        /// <item><see cref="CountCache{TEntity}"/></item>
        /// </list>
        /// </summary>
        /// <typeparam name="TEntity">The Entity/Model Type</typeparam>
        /// <returns></returns>
        public EntityFrameworkServiceBuilder<TContext> AddEntityFrameworkService<TEntity>()
            where TEntity : class
        {
            _builder.Services.TryAddScoped<EntityFrameworkServiceDependencies<TContext, TEntity>>();

            _builder.Services.TryAddScoped<EntityFrameworkService<TContext, TEntity>>();
            _builder.Services.TryAddScoped<ICrudService<TEntity>>(provider =>
                provider.GetService<EntityFrameworkService<TContext, TEntity>>());

            _builder.Services.TryAddScoped<CountCache<TEntity>>();
            return this;
        }
    

    /// <summary>
    /// Adds an individual <see cref="EntityFrameworkService{TContext, TEntity}"/>.  Note:
    /// This method configures DI for the following services:
    /// <list type="bullet">
    /// <item><see cref="EntityFrameworkServiceDependencies{TContext, TEntity}"/></item>
    /// <item><see cref="EntityFrameworkService{TContext, TEntity}"/></item>
    /// <item><see cref="CountCache{TEntity}"/></item>
    /// </list>
    /// </summary>
    /// <typeparam name="TService">The CrudService Type</typeparam>
    /// <typeparam name="TEntity">The Entity/Model Type</typeparam>
    /// <returns></returns>
    public EntityFrameworkServiceBuilder<TContext> AddEntityFrameworkService<TService,TEntity>()
        where TEntity : class
        where TService : EntityFrameworkService<TContext,TEntity>
    {
        _builder.Services.TryAddScoped<EntityFrameworkServiceDependencies<TContext, TEntity>>();

        _builder.Services.TryAddScoped<TService>();
        _builder.Services.TryAddScoped<ICrudService<TEntity>>(provider =>
            provider.GetService<TService>());

        _builder.Services.TryAddScoped<CountCache<TEntity>>();
        return this;
    }
}


}
