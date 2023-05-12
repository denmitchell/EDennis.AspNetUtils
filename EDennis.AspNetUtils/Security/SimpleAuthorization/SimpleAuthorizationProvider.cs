using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace EDennis.AspNetUtils
{
    public class SimpleAuthorizationProvider : ISimpleAuthorizationProvider
    {

        private readonly ICrudService<AppUser> _userService;
        private readonly SecurityOptions _securityOptions;

        public RolesCache _rolesCache;

        public string UserName { get; set; }

        public SimpleAuthorizationProvider(IOptionsMonitor<SecurityOptions> securityOptions,
            ICrudService<AppUser> userService, RolesCache rolesCache)
        {
            _userService = userService;
            _rolesCache = rolesCache;
            _securityOptions = securityOptions.CurrentValue;
        }


        public void UpdateClaimsPrincipal(ClaimsPrincipal principal)
        {
            if (principal != null)
            {
                if (principal.Claims.Any(c => c.Type == "role"))
                    return;

                UserName = principal.Claims.FirstOrDefault(c =>
                               c.Type.Equals(_securityOptions.IdpUserNameClaim,
                               StringComparison.OrdinalIgnoreCase))?.Value;

                if (UserName != null)
                {
                    var role = GetRole();

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

        public string GetRole()
        {
            if (!_rolesCache.TryGetValue(UserName,
                out (DateTime ExpiresAt, string Role) entry)
                || entry.ExpiresAt <= DateTime.Now)
            {

                //note: this hangs if you call await ... FirstOrDefaultAsync
                (List<dynamic> result, int _) = _userService
                    .GetAsync(select: "Role", where: "UserName == @0", new object[] { UserName })
                    .Result;

                var role = result.FirstOrDefault() as string;

                if (role == default)
                    return "undefined"; //don't cache this

                entry = (DateTime.Now.AddMilliseconds(
                    _securityOptions.RefreshInterval), role);
                _rolesCache.AddOrUpdate(UserName, entry, (u, e) => entry);

            }
            return entry.Role;
        }


    }
}
