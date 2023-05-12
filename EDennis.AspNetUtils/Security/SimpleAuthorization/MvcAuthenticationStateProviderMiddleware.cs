using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// This middleware retrieves the application role assigned to a 
    /// user and adds the role to the user's claims.
    /// This middleware is designed to be run after authentication and 
    /// authorization middleware.
    /// </summary>
    /// <typeparam name="TAppUserRolesDbContext"></typeparam>
    public class MvcAuthenticationStateProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SecurityOptions _securityOptions;
        private readonly RolesCache _rolesCache;

        /// <summary>
        /// Constructs a new instance of <see cref="MvcAuthenticationStateProviderMiddleware"/>
        /// with the provided options and roles cache.
        /// </summary>
        /// <param name="next">A delegate to invoke the next middleware in the pipeline</param>
        /// <param name="securityOptions">Options for security, including which claim type to use for user name</param>
        /// <param name="rolesCache">A temporary cache of user/role assignments</param>
        public MvcAuthenticationStateProviderMiddleware(RequestDelegate next,
            IOptionsMonitor<SecurityOptions> securityOptions,
            RolesCache rolesCache)
        {
            _next = next;
            _securityOptions = securityOptions.CurrentValue;
            _rolesCache = rolesCache;
        }

        /// <summary>
        /// Invokes the middleware.  In this case, the middleware retrieves the role for
        /// the current user and adds the role to the user's claims
        /// </summary>
        /// <param name="context">A reference to the HttpContext</param>
        /// <param name="authStateProvider">The authentication state provider (in this case,
        /// it should be <see cref="MvcAuthenticationStateProvider"/></param>
        /// <param name="appUserRolesDbContext">The DbContext used for retrieving the user's role from the database</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, MvcAuthenticationStateProvider authStateProvider,
                        ICrudService<AppUser> userService)
        {
            if (context.User != null)
            {


                var userName = context.User.Claims.FirstOrDefault(c =>
                               c.Type.Equals(_securityOptions.IdpUserNameClaim,
                               StringComparison.OrdinalIgnoreCase))?.Value;

                if (userName != null)
                {
                    var role = GetRole(userName, userService);


                    Claim[] claims;

                    if (_securityOptions.AllowMultipleRoles)
                    {
                        claims = role.Split(',').SelectMany(r => new Claim[]
                            {
                                new Claim("role", r),
                                new Claim(ClaimTypes.Role, r)
                            }).ToArray();

                    }
                    else
                    {
                        claims = new Claim[]
                            {
                                new Claim("role", role),
                                new Claim(ClaimTypes.Role, role)
                            };
                    }

                    (context.User.Identity as ClaimsIdentity).AddClaims(claims);

                    authStateProvider.User = context.User;

                    var authState = await authStateProvider.GetAuthenticationStateAsync();
                }
            }

            await _next(context);
        }

        /// <summary>
        /// Does the work of retrieving the user's role from the database
        /// </summary>
        /// <param name="userName">The user name of the user</param>
        /// <param name="appUserRolesDbContext">The DbContext used to retrieve the user's role</param>
        /// <returns></returns>
        private string GetRole(string userName, ICrudService<AppUser> userService)
        {

            if (!_rolesCache.TryGetValue(userName,
                out (DateTime ExpiresAt, string Role) entry)
                || entry.ExpiresAt <= DateTime.Now)
            {

                //note: this hangs if you call await ... FirstOrDefaultAsync
                (List<dynamic> result, int _) = userService
                    .GetAsync(select: "Role", where: "UserName == @0", new object[] { userName })
                    .Result;

                var role = result.FirstOrDefault() as string;


                if (role == default)
                    return "undefined"; //don't cache this

                entry = (DateTime.Now.AddMilliseconds(
                    _securityOptions.RefreshInterval), role);
                _rolesCache.AddOrUpdate(userName, entry, (u, e) => entry);
            }

            return entry.Role;
        }

    }

    /// <summary>
    /// Adds middleware to the pipeline.
    /// </summary>
    public static class MvcAuthenticationProviderMiddlewareExtensions
    {
        public static IApplicationBuilder UseMvcAuthenticationProvider(this IApplicationBuilder app)
        {
            app.UseMiddleware<MvcAuthenticationStateProviderMiddleware>();
            return app;
        }
    }
}
