using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Data;
using System.Security.Claims;

namespace EDennis.AspNetUtils
{
    public class SimpleAuthorizationProvider : ISimpleAuthorizationProvider
    {

        private readonly ICrudService<AppUser> _userService;
        private readonly SecurityOptions _securityOptions;

        public RolesCache _rolesCache;

        public UserNameProvider UserNameProvider { get; }
        public SimpleAuthorizationProvider(IOptionsMonitor<SecurityOptions> securityOptions,
            ICrudService<AppUser> userService, RolesCache rolesCache,
            UserNameProvider userNameProvider)
        {
            _userService = userService;
            _rolesCache = rolesCache;
            _securityOptions = securityOptions.CurrentValue;
            UserNameProvider = userNameProvider;
        }


        public async Task UpdateClaimsPrincipalAsync(ClaimsPrincipal principal)
        {
            if (principal != null)
            {
                if (principal.Claims.Any(c => c.Type == "role" || c.Type == ClaimTypes.Role))
                    return;

                UserNameProvider.UserName = principal.Claims.FirstOrDefault(c =>
                       c.Type.Equals(_securityOptions.IdpUserNameClaim,
                       StringComparison.OrdinalIgnoreCase))?.Value;

                if (UserNameProvider.UserName != null)
                {
                    var role = await GetRoleAsync();

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

                    (principal.Identity as ClaimsIdentity).AddClaims(claims);
                }
            }

        }

        public async Task<string> GetRoleAsync()
        {
            if (!_rolesCache.TryGetValue(UserNameProvider.UserName,
                out (DateTime ExpiresAt, string Role) entry)
                || entry.ExpiresAt <= DateTime.Now)
            {

                //note: this hangs if you call await ... FirstOrDefaultAsync
                (List<dynamic> result, int _) = await _userService
                    .GetAsync(select: "Role", where: "UserName == @0", new object[] { UserNameProvider.UserName });

                var role = result.FirstOrDefault() as string;

                if (role == default)
                    return "undefined"; //don't cache this

                entry = (DateTime.Now.AddMilliseconds(
                    _securityOptions.RefreshInterval), role);
                _rolesCache.AddOrUpdate(UserNameProvider.UserName, entry, (u, e) => entry);

            }
            return entry.Role;
        }


    }
}
