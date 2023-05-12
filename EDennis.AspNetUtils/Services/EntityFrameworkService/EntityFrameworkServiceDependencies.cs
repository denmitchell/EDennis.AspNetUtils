using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// A set of bundled services/objects supporting <see cref="EntityFrameworkService{TContext, TEntity}"/>.
    /// By bundling the services, CrudService implementation classes become a little more streamlined.
    /// </summary>
    /// <typeparam name="TContext">The DbContext class</typeparam>
    /// <typeparam name="TEntity">The model class</typeparam>
    public class EntityFrameworkServiceDependencies<TContext, TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        /// <summary>
        /// A service that creates and provides a DbContext.  The service also supports
        /// replacing the existing/regular DbContext with another DbContext that is
        /// more suitable for testing (e.g., one with an open transaction)
        /// </summary>
        public DbContextService<TContext> DbContextService { get; set; }

        /// <summary>
        /// A cache for holding the count of records across pages for a specific
        /// filter/where predicate.  This is helpful for preventing recounting
        /// records while a user pages through those records.
        /// </summary>
        public CountCache<TEntity> CountCache { get; set; }

        /// <summary>
        /// Provides the authenticated claims principal.  Note that MVC and Blazor Server
        /// have separate implementations for this:
        /// <list type="table">
        /// <listheader>
        /// <term>Project Type</term>
        /// <term>Implementation Class</term>
        /// </listheader>
        /// <item>
        /// <term>Blazor Server</term>
        /// <term><see cref="BlazorAuthenticationStateProvider{TAppUserRolesDbContext}"/></term>
        /// </item>
        /// <item>
        /// <term>MVC</term>
        /// <term><see cref="MvcAuthenticationStateProvider"/></term>
        /// </item>
        /// </list>
        /// </summary>
        public IAuthenticationStateProvider AuthenticationStateProvider { get; set; }

        /// <summary>
        /// Holds configurations for security such as:
        /// <list type="bullet">
        /// <item>IdpUserNameClaim -- the claim type from Azure used as the UserName</item>
        /// <item>TablePrefix -- A prefix for AppUser and AppRole tables</item>
        /// <item>Refresh Interal -- How long roles should be cached before being refreshed from the Db</item>
        /// </list>
        /// </summary>
        public SecurityOptions SecurityOptions { get; set; }

        /// <summary>
        /// A reference to the configuration object
        /// </summary>
        public IConfiguration Configuration { get; set; }


        /// <summary>
        /// Constructs a new <see cref="CrudServiceDependencies{TContext, TEntity, TAu}" object
        /// with the provided/injected service/>
        /// </summary>
        /// <param name="dbContextService">The service for creating a DbContext</param>
        /// <param name="authStateProvider">Provides access to the authenticated user</param>
        /// <param name="securityOptions">Security options</param>
        /// <param name="countCache">A cache for record counts across pages</param>
        /// <param name="config">A reference to the configuation object</param>
        public EntityFrameworkServiceDependencies(DbContextService<TContext> dbContextService,
            IAuthenticationStateProvider authStateProvider,
            IOptionsMonitor<SecurityOptions> securityOptions,
            CountCache<TEntity> countCache,
            IConfiguration config)
        {

            DbContextService = dbContextService;
            AuthenticationStateProvider = authStateProvider;
            SecurityOptions = securityOptions.CurrentValue;
            CountCache = countCache;
            Configuration = config;
        }


        /// <summary>
        /// Gets a test instance of <see cref="EntityFrameworkServiceDependencies{TContext, TEntity}"/>
        /// </summary>
        /// <param name="config">A reference to the configuration object</param>
        /// <param name="userName">The user name to hard code for the test</param>
        /// <param name="role">The role to hard code for the test</param>
        /// <returns></returns>
        public static EntityFrameworkServiceDependencies<TContext, TEntity> GetTestInstance(IConfiguration config, string userName, string role)
        {

            var securityOptions = config.GetOrThrow<SecurityOptions>("Security");
            var iomSecurityOptions = new OptionsMonitor<SecurityOptions>(securityOptions);

            var CountCache = new CountCache<TEntity>();
            var authStateProvider = new TestAuthenticationStateProvider(userName, role);
            var dbContextService = new DbContextService<TContext>(config);

            return new EntityFrameworkServiceDependencies<TContext, TEntity>(
                dbContextService, authStateProvider, iomSecurityOptions, CountCache, config);

        }

        /// <summary>
        /// An implementation of <see cref="IAuthenticationStateProvider"/> suitable
        /// for testing
        /// </summary>
        class TestAuthenticationStateProvider : IAuthenticationStateProvider
        {
            public string IdpUserNameClaim { get; }
            public ClaimsPrincipal User { get; set; }

            public TestAuthenticationStateProvider(string userName, string role,
                string idpUserNameClaim = "preferred_username")
            {
                IdpUserNameClaim = idpUserNameClaim;
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(new Claim[]
                    {
                    new Claim(IdpUserNameClaim,userName),
                    new Claim(ClaimTypes.Name,userName),
                    new Claim("name",userName),
                    new Claim("role",role),
                    new Claim(ClaimTypes.Role,role),
                    }));
            }

            public Task<AuthenticationState> GetAuthenticationStateAsync()
                => Task.FromResult(new AuthenticationState(User));

        }


    }
}
