using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Interface supporting <see cref="BlazorAuthenticationStateProvider{TAppUserRolesDbContext}"/>
    /// and <see cref="MvcAuthenticationStateProvider"/>
    /// </summary>
    public interface IAuthenticationStateProvider
    {
        /// <summary>
        /// The authenticated ClaimsPrincipal
        /// </summary>
        ClaimsPrincipal User { get; set; }

        /// <summary>
        /// Used to obtain the authenticated user (claims principal)
        /// </summary>
        /// <returns></returns>
        Task<AuthenticationState> GetAuthenticationStateAsync()
            => Task.FromResult(new AuthenticationState(User));
    }
}
