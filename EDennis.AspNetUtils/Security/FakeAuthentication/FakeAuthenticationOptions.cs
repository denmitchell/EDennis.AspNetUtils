using Microsoft.AspNetCore.Authentication;


namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Configuration options for Fake Authentication. Since this is
    /// fake authentication (used for testing), the options are 
    /// hard-coded for easy.
    /// </summary>
    public class FakeAuthenticationOptions : AuthenticationSchemeOptions
    {
        public readonly static string ConfigurationKey = "FakeUser";
        public readonly static string AccessDefinedPath = "/Forbidden/";
        public readonly static TimeSpan CookieLifeTime = TimeSpan.FromDays(1);
        public readonly static bool CookieSlidingExpiration = true;

    }
}
