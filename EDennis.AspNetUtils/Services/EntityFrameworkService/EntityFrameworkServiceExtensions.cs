using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EDennis.AspNetUtils
{
    public static class EntityFrameworkServiceExtensions
    {
        /// <summary>
        /// This extension method sets up <see cref="EntityFrameworkService{TContext, TEntity}"/> implementations
        /// for <typeparamref name="TContext"/>.  The extension method also configures DI for
        /// <see cref="DbContextService{TContext}"/>.  The extension method is designed to be followed
        /// upon by <see cref="EntityFrameworkServiceBuilder{TContext}.AddEntityFrameworkService{TService, TEntity}"/>
        /// for various individual CrudServices for each entity
        /// </summary>
        /// <typeparam name="TContext">The DbContext type</typeparam>
        /// <param name="builder">A reference to the <see cref="WebApplicationBuilder"/></param>
        /// <param name="sectionKey">The key to the DbContexts section holding the connection strings</param>
        /// <returns></returns>
        public static EntityFrameworkServiceBuilder<TContext> AddEntityFrameworkServices<TContext>(this WebApplicationBuilder builder,
               string sectionKey = "DbContexts")
            where TContext : DbContext
        {
            builder.Services.TryAddScoped<DbContextService<TContext>>();
            builder.Services.TryAddScoped<UserNameProvider, UserNameProvider>();

            //builder.Services.AddDbContext<TContext>(options =>
            //{
            //    DbContextService<TContext>.GetDbContextOptions(options, builder.Configuration, sectionKey);
            //});

            if (!builder.Services.Any(s => s.ServiceType == typeof(TContext)))
            {
                builder.Services.AddTransient(provider =>
                {
                    return DbContextService<TContext>.GetDbContext(builder.Configuration, sectionKey);
                });
            }
            return new EntityFrameworkServiceBuilder<TContext>(builder);
        }



    }
}
