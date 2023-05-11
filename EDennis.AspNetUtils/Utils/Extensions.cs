using EDennis.AspNetUtils.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace EDennis.AspNetUtils
{

    /// <summary>
    /// Various helpful extension methods
    /// </summary>
    public static class Extensions
    {

        /// <summary>
        /// Attempts to bind a value or object to configuration, and
        /// can throw an exception upon failure to find the key.
        /// </summary>
        /// <typeparam name="T">The target type to bind to</typeparam>
        /// <param name="config">The IConfiguration instance</param>
        /// <param name="key">The configuration key</param>
        /// <param name="checkIfDefault">whether to check for the existence
        /// of the key, if the binding process returns a default value</param>
        /// <param name="logger">The logger to use for error logging</param>
        public static T GetOrThrow<T>(this IConfiguration config,
            string key, bool checkIfDefault = true, ILogger logger = null)
        {

            T result;

            if (IsSimpleType(typeof(T)))
                //bind to simple value
                result = config.GetValue<T>(key);
            else
                //bind to object
                result = config.GetSection(key).Get<T>();

            //if value is default value && checkIfDefault == true, make sure
            //that the key exists
            if (EqualityComparer<T>.Default.Equals(result, default) && checkIfDefault)
            {
                if (config is IConfigurationRoot root)
                {
                    if (root.ContainsKey(key))
                        return result;
                    else
                        Throw(key, typeof(T), logger); //key doesn't exist
                }
            }

            return result;
        }

        /// <summary>
        /// Adds Simple Authorization -- the ability to resolve the user name of an authenticated
        /// user to an application role, where the user and role are stored in two tables of the
        /// database.
        /// NOTE: this extension method configures DI for the following services:
        /// <list type="bullet">
        /// <item><see cref="RolesCache"/></item>
        /// <item><see cref="IAuthenticationStateProvider"/></item>
        /// <item><see cref="AuthenticationStateProvider"/> (Blazor only)</item>
        /// <item><see cref="DbContextService{TContext}"/></item>
        /// <item><see cref="AppUserService{TAppUserRolesDbContext}"/></item>
        /// <item><see cref="AppRoleService{TAppUserRolesDbContext}"/></item>
        /// </list>
        /// The extension method is designed to be 
        /// </summary>
        /// <typeparam name="TAppUserRolesDbContext">The DbContext that holds the users and roles</typeparam>
        /// <param name="builder">The <see cref="WebApplicationBuilder"/></param>
        /// <param name="securityConfigKey">The configuration key for security</param>
        /// <param name="dbContextsConfigKey">The configuration key for DbContext</param>
        /// <returns></returns>
        public static WebApplicationBuilder AddSimpleAuthorization<TAppUserRolesDbContext>(this WebApplicationBuilder builder,
            bool isBlazor,
            string securityConfigKey = "Security", string dbContextsConfigKey = "DbContexts")
            where TAppUserRolesDbContext : AppUserRolesContext
        {
            builder.Services.TryAddSingleton<RolesCache>();
            if (isBlazor)
            {
                builder.Services.AddScoped<BlazorAuthenticationStateProvider<TAppUserRolesDbContext>>();
                builder.Services.TryAddScoped<IAuthenticationStateProvider>(provider =>
                    provider.GetService<BlazorAuthenticationStateProvider<TAppUserRolesDbContext>>()
                );
                builder.Services.TryAddScoped<AuthenticationStateProvider>(provider =>
                    provider.GetService<BlazorAuthenticationStateProvider<TAppUserRolesDbContext>>()
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

            if (!builder.Services.Any(s => s.ServiceType == typeof(TAppUserRolesDbContext)))
            {
                builder.Services.AddTransient(provider =>
                {
                    return DbContextService<TAppUserRolesDbContext>.GetDbContext(builder.Configuration, dbContextsConfigKey);
                });
            }
            builder.Services.TryAddScoped<DbContextService<TAppUserRolesDbContext>>();


            var ccBuilder = new CrudServiceConfigurationBuilder<TAppUserRolesDbContext>(builder);
            ccBuilder
                .AddCrudService<AppUserService<TAppUserRolesDbContext>, AppUser>()
                .AddCrudService<AppRoleService<TAppUserRolesDbContext>, AppRole>();

            return builder;
        }

        /// <summary>
        /// This extension method sets up <see cref="EntityFrameworkService{TContext, TEntity}"/> implementations
        /// for <typeparamref name="TContext"/>.  The extension method also configures DI for
        /// <see cref="DbContextService{TContext}"/>.  The extension method is designed to be followed
        /// upon by <see cref="CrudServiceConfigurationBuilder{TContext}.AddCrudService{TService, TEntity}"/>
        /// for various individual CrudServices for each entity
        /// </summary>
        /// <typeparam name="TContext">The DbContext type</typeparam>
        /// <param name="builder">A reference to the <see cref="WebApplicationBuilder"/></param>
        /// <param name="sectionKey">The key to the DbContexts section holding the connection strings</param>
        /// <returns></returns>
        public static CrudServiceConfigurationBuilder<TContext> AddCrudServices<TContext>(this WebApplicationBuilder builder,
               string sectionKey = "DbContexts")
            where TContext : DbContext
        {
            if (!builder.Services.Any(s => s.ServiceType == typeof(TContext)))
            {
                builder.Services.AddScoped(provider =>
                {
                    return DbContextService<TContext>.GetDbContext(builder.Configuration, sectionKey);
                });
            }
            builder.Services.TryAddScoped<DbContextService<TContext>>();
            return new CrudServiceConfigurationBuilder<TContext>(builder);
        }



        /// <summary>
        /// Retrieves (minified) JSON from an environment variable, parses
        /// that JSON as though it were an additional appsettings file, and
        /// adds the resulting key/value pairs to configuration
        /// </summary>
        /// <param name="builder">The configuration services</param>
        /// <param name="key">The environment variable name</param>
        /// <returns>the configuration services (for fluent construction)</returns>
        /// <exception cref="ArgumentException">when the environment variable isn't defined</exception>
        public static IConfigurationBuilder AddJsonEnvironmentVariable(this IConfigurationBuilder builder, string key, bool optional = true)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (!optional && string.IsNullOrEmpty(value))
                throw new ArgumentException($"Environment variable {key} not set.");

            if (!string.IsNullOrEmpty(value))
                builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(value ?? "")));

            return builder;
        }

        /// <summary>
        /// Adds fake user authentication using a <see cref="FakeAuthenticationHandler"/>
        /// and the <see cref="AuthenticationSchemeConstants.FakeAuthenticationScheme"/>
        /// </summary>
        /// <param name="services">a reference to the IServiceCollection instance</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddFakeUserAuthentication(this IServiceCollection services)
        {
            AuthenticationBuilder builder = null;


            builder = services.AddAuthentication(
                options =>
                {
                    options.DefaultScheme = AuthenticationSchemeConstants.FakeAuthenticationScheme;
                })
                .AddScheme<FakeAuthenticationOptions, FakeAuthenticationHandler>(
                        AuthenticationSchemeConstants.FakeAuthenticationScheme, options =>
                        {
                        })
                .AddCookie(AuthenticationSchemeConstants.FakeAuthenticationScheme + "Cookie", o =>
                {
                    o.Cookie.Name = AuthenticationSchemeConstants.FakeAuthenticationScheme;
                    o.ExpireTimeSpan = FakeAuthenticationOptions.CookieLifeTime;
                    o.SlidingExpiration = true;
                    o.AccessDeniedPath = FakeAuthenticationOptions.AccessDefinedPath;
                    o.LogoutPath = "/";
                });


            return builder;
        }

        /// <summary>
        /// Adds ApiKeyAuthentication to an API.  This is added to a project that exposes the
        /// API via a controller.
        /// </summary>
        /// <param name="services">A reference to the IServiceCollection instance</param>
        /// <param name="config">A reference to IConfiguration instance</param>
        /// <param name="configKey">The configuration key for the section (default to Security:ApiKey)</param>
        /// <param name="additionalConfigure">an optional action to configure authentication options further</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddApiKeyAuthentication(this IServiceCollection services,
            IConfiguration config, string configKey = null, Action<AuthenticationOptions> additionalConfigure = null
            )
        {
            configKey ??= ApiKeyAuthenticationOptions.DefaultConfigKey;
            if (!services.Any(s => s.ServiceType == typeof(IOptionsMonitor<ApiKeyAuthenticationOptions>)))
            {
                services.BindAndConfigure(config, configKey, out ApiKeyAuthenticationOptions _);
            }

            AuthenticationBuilder builder = null;
            builder = services.AddAuthentication(
                    options =>
                    {
                        options.DefaultScheme = AuthenticationSchemeConstants.ApiKeyAuthenticationScheme;
                        additionalConfigure?.Invoke(options);
                    })
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                    AuthenticationSchemeConstants.ApiKeyAuthenticationScheme, options => { });

            return builder;
        }





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
        public static WebApplicationBuilder AddApiKeyClients(this WebApplicationBuilder builder,
            string configKey = "Apis", string securityConfigKey = "Security")
        {
            //get the settings from configuration and also setup
            //DI to inject IOptionsMonitor<ApiClientSettingsDictionary>
            builder.Services.BindAndConfigure(builder.Configuration, configKey, out ApiClientSettingsDictionary apis);


            if (!builder.Services.Any(s => s.ServiceType == typeof(IOptionsMonitor<SecurityOptions>)))
            {
                builder.Services.BindAndConfigure(builder.Configuration, securityConfigKey, out SecurityOptions _);
            }


            //interate over all API settings in configuration
            foreach (var api in apis)
            {
                //configure DI for a named HttpClient, where the name is 
                //the configuration key for the individual API
                IHttpClientBuilder httpClientBuilder = builder.Services.AddHttpClient(api.Key, configure =>
                {
                    configure.BaseAddress = new Uri(api.Value.BaseAddress);
                });
                httpClientBuilder.AddHttpMessageHandler<ApiKeyMessageHandler>();
            }

            builder.Services.AddTransient<ApiKeyMessageHandler>();
            if (!builder.Services.Any(s => s.ServiceType == typeof(IHttpContextAccessor)))
                builder.Services.AddHttpContextAccessor();

            return builder;
        }





        /// <summary>
        /// Binds and configures options for a particular type.  The
        /// deserialized options are returned through the settings out parameter.
        /// The options are configured in DI so that the DI container can
        /// inject instances of IOptionsMonitor{T}.
        /// </summary>
        /// <typeparam name="T">The model class for the settings</typeparam>
        /// <param name="services">The IServiceCollection instance</param>
        /// <param name="config">The IConfiguration instance</param>
        /// <param name="configKey">The key to the relevant section of configuration</param>
        /// <param name="settings">The resulting settings object after binding</param>
        /// <returns></returns>
        public static IServiceCollection BindAndConfigure<T>(this IServiceCollection services, IConfiguration config, string configKey, out T settings)
            where T : class, new()
        {
            settings = new T();
            var configSection = config.GetSection(configKey);
            configSection.Bind(settings);

            services.Configure<T>(configSection.Bind);

            return services;
        }


        /// <summary>
        /// Whether a configuration key exists
        /// </summary>
        /// <param name="config">The root configuration (downcast from IConfiguration,
        /// if needed)</param>
        /// <param name="key">The key to check</param>
        /// <returns></returns>
        public static bool ContainsKey(this IConfigurationRoot config, string key)
        {

            //get all the provider sources contributing to configuration
            var providers = config.Providers.ToList();

            //iterate over the provider sources, looking for the key
            for (var i = 0; i < providers.Count; i++)
            {
                var provider = providers[i];
                //if the key exists for a parent section, return true
                if (provider.GetChildKeys(Array.Empty<string>(), key).Any())
                {
                    return true;
                    //if the key exists for a value, return true
                }
                else if (provider.TryGet(key, out string _))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Helper method for throwing and logging an exception 
        /// </summary>
        /// <param name="key">The configuation key to log</param>
        /// <param name="type">The target object's type</param>
        /// <param name="logger">The logger to log to</param>
        /// <returns></returns>
        private static string Throw(string key, Type type, ILogger logger = null)
        {
            var ex = new ApplicationException($"Could not bind key '{key}' " +
                $"in Configuration to object of type {CSharpName(type)}");

            if (logger != null)
                logger.LogError(ex, ex.Message);

            throw ex;
        }



        /// <summary>
        /// from https://stackoverflow.com/a/21269486
        /// Get full type name with full namespace names
        /// </summary>
        /// <param name="type">
        /// The type to get the C# name for (may be a generic type or a nullable type).
        /// </param>
        /// <returns>
        /// Full type name, fully qualified namespaces
        /// </returns>
        public static string CSharpName(this Type type)
        {
            Type nullableType = Nullable.GetUnderlyingType(type);
            string nullableText;
            if (nullableType != null)
            {
                type = nullableType;
                nullableText = "?";
            }
            else
            {
                nullableText = string.Empty;
            }

            if (type.IsGenericType)
            {
                return string.Format(
                    "{0}<{1}>{2}",
                    type.Name[..type.Name.IndexOf('`')],
                    string.Join(", ", type.GetGenericArguments().Select(ga => ga.CSharpName())),
                    nullableText);
            }

            return type.Name switch
            {
                "String" => "string",
                "Int32" => "int" + nullableText,
                "Decimal" => "decimal" + nullableText,
                "Object" => "object" + nullableText,
                "Void" => "void" + nullableText,
                _ => (string.IsNullOrWhiteSpace(type.FullName) ? type.Name : type.FullName) + nullableText,
            };
        }


        /// <summary>
        /// From System.Data.Linq
        /// Returns true if the type is one of the built in simple types.
        /// </summary>
        internal static bool IsSimpleType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = type.GetGenericArguments()[0];

            if (type.IsEnum)
                return true;

            if (type == typeof(Guid))
                return true;

            TypeCode tc = Type.GetTypeCode(type);
            switch (tc)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Char:
                case TypeCode.String:
                case TypeCode.Boolean:
                case TypeCode.DateTime:
                    return true;
                case TypeCode.Object:
                    return (typeof(TimeSpan) == type) || (typeof(DateTimeOffset) == type);
                default:
                    return false;
            }
        }

    }

}
