using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Extends Blazor's <see cref="ServerAuthenticationStateProvider"/>, overriding
    /// <see cref="GetAuthenticationStateAsync"/> in order to retrieve application
    /// roles associated with the authenticated user for the current Blazor application.
    /// </summary>
    /// <typeparam name="TAppUserRolesDbContext">The DbContext used to retrieve roles from the database</typeparam>
    public class BlazorAuthenticationStateProvider
        : ServerAuthenticationStateProvider
    {

        private readonly ISimpleAuthorizationProvider _authProvider;

        /// <summary>
        /// Constructs a new <see cref="BlazorAuthenticationStateProvider"/>
        /// with the provided services and options
        /// </summary>
        /// <param name="authProvider"><see cref="ISimpleAuthorizationProvider"/></param>
        public BlazorAuthenticationStateProvider(ISimpleAuthorizationProvider authProvider)
        {
            _authProvider = authProvider;
        }

        /// <summary>
        /// Gets the authentication state of the user.  Also sets the role claim
        /// </summary>
        /// <returns></returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var authState = await base.GetAuthenticationStateAsync();

            if (authState?.User != null)
                _authProvider.UpdateClaimsPrincipal(authState.User);

            return authState;
        }

    }
}
