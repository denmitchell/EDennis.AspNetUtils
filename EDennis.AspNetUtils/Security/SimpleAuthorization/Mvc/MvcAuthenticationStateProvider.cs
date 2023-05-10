using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// A class that holds the ClaimsPrincipal and its claims, including user name
    /// and role.  This implementation of <see cref="IAuthenticationStateProvider"/>
    /// can be used in applications that route all requests through an HTTP pipeline,
    /// where <see cref="MvcAuthenticationStateProviderMiddleware{TAppUserRolesDbContext}"/>
    /// could be activated on each request.
    /// </summary>
    public class MvcAuthenticationStateProvider : IAuthenticationStateProvider
    {
        /// <summary>
        /// The authenticated ClaimsPrincipal
        /// </summary>
        public ClaimsPrincipal User { get; set; }

        /// <summary>
        /// Gets the authentication state of the ClaimsPrincipal.  For this implementation,
        /// it merely wraps the <see cref="User"/>.
        /// </summary>
        /// <returns></returns>
        public virtual Task<AuthenticationState> GetAuthenticationStateAsync()
            => Task.FromResult(new AuthenticationState(User));
    }
}
