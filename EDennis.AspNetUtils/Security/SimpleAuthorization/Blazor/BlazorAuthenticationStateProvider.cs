using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Extends Blazor's <see cref="ServerAuthenticationStateProvider"/>, overriding
    /// <see cref="GetAuthenticationStateAsync"/> in order to retrieve application
    /// roles associated with the authenticated user for the current Blazor application.
    /// </summary>
    /// <typeparam name="TAppUserRolesDbContext">The DbContext used to retrieve roles from the database</typeparam>
    public class BlazorAuthenticationStateProvider<TAppUserRolesDbContext> 
        : ServerAuthenticationStateProvider, IAuthenticationStateProvider
        where TAppUserRolesDbContext : AppUserRolesContext
    {

        /// <summary>
        /// Returns the ClaimsPrincipal.  
        /// NOTE: set is not implemented here
        /// </summary>
        public ClaimsPrincipal User 
        { 
            get 
            {
                return base.GetAuthenticationStateAsync().Result.User;
            } set {
                throw new NotImplementedException("User is readonly for Blazor");
            }
        }

        private readonly SecurityOptions _securityOptions;
        private readonly RolesCache _rolesCache;
        private readonly TAppUserRolesDbContext _appUserRolesDbContext;

        /// <summary>
        /// Constructs a new <see cref="BlazorAuthenticationStateProvider{TAppUserRolesDbContext}"/>
        /// with the provided services and options
        /// </summary>
        /// <param name="appUserRolesDbContext">The DbContext used to register users and assign roles</param>
        /// <param name="securityOptions">Basic options for security, including the claim used as the UserName</param>
        /// <param name="rolesCache">A temporary cache of roles assigned to the user.</param>
        public BlazorAuthenticationStateProvider(
            TAppUserRolesDbContext appUserRolesDbContext,
            IOptionsMonitor<SecurityOptions> securityOptions,
            RolesCache rolesCache) {
            _appUserRolesDbContext = appUserRolesDbContext;
            _securityOptions = securityOptions.CurrentValue;
            _rolesCache = rolesCache;
        }

        /// <summary>
        /// Gets the authentication state of the user.  Also sets the role claim
        /// </summary>
        /// <returns></returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var authState =  await base.GetAuthenticationStateAsync();

            if (!authState.User.Claims.Any(c => c.Type == "role"))
            {
                var userName = authState.User.Claims.FirstOrDefault(c =>
                    c.Type.Equals(_securityOptions.IdpUserNameClaim,
                    StringComparison.OrdinalIgnoreCase))?.Value;

                if (userName != null)
                {
                    var role = GetRole(userName);

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

                    (authState.User.Identity as ClaimsIdentity).AddClaims(claims);

                }
            }

            return authState;
        }

        /// <summary>
        /// Gets the role for the user from the database or from cache
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private string GetRole(string userName)
        {

            if (!_rolesCache.TryGetValue(userName,
                out (DateTime ExpiresAt, string Role) entry)
                || entry.ExpiresAt <= DateTime.Now)
            {
                    //note: this hangs if you call await ... FirstOrDefaultAsync
                    var role = _appUserRolesDbContext.AppUsers
                                .Where(u => u.UserName == userName)
                                .Select(u => u.Role)
                                .FirstOrDefault();

                    if (role == default)
                        return "undefined"; //don't cache this

                    entry = (DateTime.Now.AddMilliseconds(
                        _securityOptions.RefreshInterval), role);
                    _rolesCache.AddOrUpdate(userName, entry, (u, e) => entry);


            }

            return entry.Role;
        }



    }
}
