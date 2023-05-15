using System.Security.Claims;

namespace EDennis.AspNetUtils
{
    public interface ISimpleAuthorizationProvider
    {
        Task<string> GetRoleAsync();
        UserNameProvider UserNameProvider { get; }
        Task UpdateClaimsPrincipalAsync(ClaimsPrincipal principal);
    }
}