﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

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

        /// <summary>
        /// Constructs a new instance of <see cref="MvcAuthenticationStateProviderMiddleware"/>
        /// with the provided options and roles cache.
        /// </summary>
        /// <param name="next">A delegate to invoke the next middleware in the pipeline</param>
        public MvcAuthenticationStateProviderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invokes the middleware.  In this case, the middleware retrieves the role for
        /// the current user and adds the role to the user's claims
        /// </summary>
        /// <param name="context">A reference to the HttpContext</param>
        /// <param name="authProvider"><see cref="ISimpleAuthorizationProvider"/> instance</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, ISimpleAuthorizationProvider authProvider)
        {
            authProvider.UpdateClaimsPrincipalAsync(context.User);
            await _next(context);
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
