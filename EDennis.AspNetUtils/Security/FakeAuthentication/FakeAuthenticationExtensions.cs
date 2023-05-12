using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace EDennis.AspNetUtils
{
    public static class FakeAuthenticationExtensions
    {
        /// <summary>
        /// Adds fake user authentication using a <see cref="FakeAuthenticationHandler"/>
        /// and the <see cref="AuthenticationSchemeConstants.FakeAuthenticationScheme"/>
        /// </summary>
        /// <param name="services">a reference to the IServiceCollection instance</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddFakeAuthentication(this IServiceCollection services)
        {
            AuthenticationBuilder builder = null;


            builder = services.AddAuthentication(
                options =>
                {
                    options.DefaultScheme = AuthenticationSchemeConstants.FakeAuthenticationScheme;
                })
                .AddScheme<FakeAuthenticationOptions, FakeAuthenticationHandler>(
                        AuthenticationSchemeConstants.FakeAuthenticationScheme, options =>
                        {
                        })
                .AddCookie(AuthenticationSchemeConstants.FakeAuthenticationScheme + "Cookie", o =>
                {
                    o.Cookie.Name = AuthenticationSchemeConstants.FakeAuthenticationScheme;
                    o.ExpireTimeSpan = FakeAuthenticationOptions.CookieLifeTime;
                    o.SlidingExpiration = true;
                    o.AccessDeniedPath = FakeAuthenticationOptions.AccessDefinedPath;
                    o.LogoutPath = "/";
                });


            return builder;
        }


    }
}
