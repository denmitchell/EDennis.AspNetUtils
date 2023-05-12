using System.Security.Claims;

namespace EDennis.AspNetUtils
{
    public interface ISimpleAuthorizationProvider
    {
        string GetRole();
        UserNameProvider UserNameProvider { get; }
        void UpdateClaimsPrincipal(ClaimsPrincipal principal);
    }
}