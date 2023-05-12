using System.Security.Claims;

namespace EDennis.AspNetUtils
{
    public interface ISimpleAuthorizationProvider
    {
        string UserName { get; set; }

        string GetRole();
        void UpdateClaimsPrincipal(ClaimsPrincipal principal);
    }
}