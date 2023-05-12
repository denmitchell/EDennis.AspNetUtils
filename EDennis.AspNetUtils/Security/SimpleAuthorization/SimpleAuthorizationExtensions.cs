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
        /// <item><see cref="ISimpleAuthorizationProvider"/></item>
        /// <item><see cref="AuthenticationStateProvider"/> (Blazor only)</item>
        /// </list>
        /// The extension method is designed to be 
        /// </summary>
        /// <typeparam name="TRoleServiceImpl">implementation of <see cref="EntityFrameworkService{SimpleAuthContext, AppUser}"/></typeparam>
        /// <typeparam name="TUserServiceImpl">implementation of <see cref="EntityFrameworkService{SimpleAuthContext, AppRole}"/></typeparam>
        /// <param name="builder">The <see cref="WebApplicationBuilder"/></param>
        /// <param name="isBlazorServer">If this is a Blazor Configuration</param>
        /// <param name="securityConfigKey">The configuration key for Security</param>
        /// <param name="dbContextConfigKey">The configuration key for DbContexts</param>
        /// <returns></returns>
        public static WebApplicationBuilder AddSimpleAuthorization<TUserServiceImpl, TRoleServiceImpl>(
            this WebApplicationBuilder builder, bool isBlazorServer, string securityConfigKey = "Security", 
            string dbContextConfigKey = null)
            where TUserServiceImpl : EntityFrameworkService<SimpleAuthContext, AppUser>
            where TRoleServiceImpl : EntityFrameworkService<SimpleAuthContext, AppRole>
        {

            AddSecurity(builder, isBlazorServer, securityConfigKey);

            dbContextConfigKey ??= "DbContexts";
                builder.AddEntityFrameworkServices<SimpleAuthContext>(dbContextConfigKey)
                    .AddEntityFrameworkService<TUserServiceImpl,AppUser>()
                    .AddEntityFrameworkService<TRoleServiceImpl,AppRole>();

            return builder;
        }


        /// <summary>
        /// Adds Simple Authorization -- the ability to resolve the user name of an authenticated
        /// user to an application role, where the user and role are stored in two tables of the
        /// database.
        /// NOTE: this extension method configures DI for the following services:
        /// <list type="bullet">
        /// <item><see cref="RolesCache"/></item>
        /// <item><see cref="ISimpleAuthorizationProvider"/></item>
        /// <item><see cref="AuthenticationStateProvider"/> (Blazor Server only)</item>
        /// </list>
        /// The extension method is designed to be 
        /// </summary>
        /// <typeparam name="TRoleServiceImpl">implementation of <see cref="ApiClientService{AppUser}"/></typeparam>
        /// <typeparam name="TUserServiceImpl">implementation of <see cref="ApiClientService{AppRole}"/></typeparam>
        /// <param name="builder">The <see cref="WebApplicationBuilder"/></param>
        /// <param name="isBlazorServer">If this is a Blazor Configuration</param>
        /// <param name="securityConfigKey">The configuration key for Security</param>
        /// <param name="apiSettingsConfigKey">The configuration key for Apis</param>
        /// <returns></returns>
        public static WebApplicationBuilder AddSimpleAuthorization<TUserServiceImpl, TRoleServiceImpl>(this WebApplicationBuilder builder,
            bool isBlazorServer, string securityConfigKey = "Security", string apiSettingsConfigKey = null,
            bool? addApiKeyMessageHandler = null)
            where TUserServiceImpl : ApiClientService<AppUser>
            where TRoleServiceImpl : ApiClientService<AppRole>
        {

            AddSecurity(builder, isBlazorServer, securityConfigKey);

            apiSettingsConfigKey ??= "Apis";
            addApiKeyMessageHandler ??= true;
            builder.AddApiClientServices(addApiKeyMessageHandler.Value, apiSettingsConfigKey, securityConfigKey)
                .AddApiClientService<AppUser>()
                .AddApiClientService<AppRole>();

            return builder;
        }

        /// <summary>
        /// Configures DI for security classes:
        /// <list type="bullent">
        /// <item><see cref="RolesCache"/></item>
        /// <item><see cref="ISimpleAuthorizationProvider"/></item>
        /// <item><see cref="AuthenticationStateProvider"/> (Blazor Server only)</item>
        /// </list>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="isBlazor"></param>
        /// <param name="securityConfigKey"></param>
        private static void AddSecurity(WebApplicationBuilder builder, bool isBlazor, string securityConfigKey = "Security" )
        {
            var defaultType = builder.Services
                .FirstOrDefault(s => s.ServiceType == typeof(AuthenticationStateProvider))
                ?.ServiceType;
            if (defaultType != null)
                builder.Services.RemoveAll(defaultType);

            if (!builder.Services.Any(s => s.ServiceType == typeof(IOptionsMonitor<SecurityOptions>)))
                builder.Services.BindAndConfigure(builder.Configuration, securityConfigKey, out SecurityOptions _);

            builder.Services.AddSingleton<RolesCache>();
            builder.Services.AddScoped<ISimpleAuthorizationProvider, SimpleAuthorizationProvider>();

            if (isBlazor)
                builder.Services.AddScoped<AuthenticationStateProvider, BlazorAuthenticationStateProvider>();

        }

    }
}
