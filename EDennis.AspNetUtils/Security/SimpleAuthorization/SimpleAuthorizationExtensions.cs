using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EDennis.AspNetUtils
{
    public static class SimpleAuthorizationExtensions
    {

        /// <summary>
        /// Adds Simple Authorization -- the ability to resolve the user name of an authenticated
        /// user to an application role, where the user and role are stored in two tables of the
        /// database.
        /// NOTE: this extension method configures DI for the following services:
        /// <list type="bullet">
        /// <item><see cref="RolesCache"/></item>
        /// <item><see cref="IAuthenticationStateProvider"/></item>
        /// <item><see cref="AuthenticationStateProvider"/> (Blazor only)</item>
        /// </list>
        /// The extension method is designed to be 
        /// </summary>
        /// <typeparam name="TCrudServiceImpl">either <see cref="EntityFrameworkService{TContext, TEntity}"/>
        /// or <see cref="ApiClientService{TEntity}"/></typeparam>
        /// <param name="builder">The <see cref="WebApplicationBuilder"/></param>
        /// <param name="securityConfigKey">The configuration key for security</param>
        /// <param name="serviceSettingsConfigKey">The configuration key for DbContexts or Apis</param>
        /// <returns></returns>
        public static WebApplicationBuilder AddSimpleAuthorization<TCrudServiceImpl>(this WebApplicationBuilder builder,
            bool isBlazor, string securityConfigKey = "Security", string serviceSettingsConfigKey = null,
            bool? addApiKeyMessageHandler = null)
            where TCrudServiceImpl : ICrudService<AppUser>
        {
            builder.Services.TryAddSingleton<RolesCache>();
            if (isBlazor)
            {
                builder.Services.AddScoped<BlazorAuthenticationStateProvider>();
                builder.Services.TryAddScoped<IAuthenticationStateProvider>(provider =>
                    provider.GetService<BlazorAuthenticationStateProvider>()
                );
                builder.Services.TryAddScoped<AuthenticationStateProvider>(provider =>
                    provider.GetService<BlazorAuthenticationStateProvider>()
                );
            }
            else
            {
                builder.Services.TryAddScoped<MvcAuthenticationStateProvider>();
                builder.Services.TryAddScoped<IAuthenticationStateProvider>(provider =>
                    provider.GetService<MvcAuthenticationStateProvider>()
                );
            }

            if (!builder.Services.Any(s => s.ServiceType == typeof(IOptionsMonitor<SecurityOptions>)))
            {
                builder.Services.BindAndConfigure(builder.Configuration, securityConfigKey, out SecurityOptions _);
            }

            if (typeof(TCrudServiceImpl) == typeof(EntityFrameworkService<SimpleAuthContext, AppUser>))
            {
                serviceSettingsConfigKey ??= "DbContexts";
                builder.AddEntityFrameworkServices<SimpleAuthContext>(serviceSettingsConfigKey)
                    .AddEntityFrameworkService<AppUser>()
                    .AddEntityFrameworkService<AppRole>();
            }
            else if (typeof(TCrudServiceImpl) == typeof(EntityFrameworkService<SimpleAuthContext, AppUser>))
            {
                serviceSettingsConfigKey ??= "Apis";
                addApiKeyMessageHandler ??= true;
                builder.AddApiClientServices(addApiKeyMessageHandler.Value, serviceSettingsConfigKey, securityConfigKey)
                    .AddApiClientService<AppUser>();
            }

            return builder;
        }

    }
}
