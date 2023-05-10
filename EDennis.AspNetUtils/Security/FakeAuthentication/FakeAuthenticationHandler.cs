using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// NOTE: This fake authentication handler is used for testing, and it is designed to
    /// require very simple configuration.  Ensure that you set a command-line argument 
    /// of FakeUser=Maria (or some other user)  
    /// </summary>
    public class FakeAuthenticationHandler : AuthenticationHandler<FakeAuthenticationOptions>
        //IAuthenticationService
    {
        private readonly IConfiguration _config;
        private readonly SecurityOptions _securityOptions;

        /// <summary>
        /// Constructs a new instance of <see cref="FakeAuthenticationHandler"/>
        /// </summary>
        /// <param name="options">Hard-coded options</param>
        /// <param name="logger">The logger factory for logging</param>
        /// <param name="encoder">A URL encoder</param>
        /// <param name="clock">A system clock for retrieving or faking UTC date/time</param>
        /// <param name="config">A reference to Configuration</param>
        /// <param name="securityOptions">Configuration options, including the username claim</param>
        public FakeAuthenticationHandler(IOptionsMonitor<FakeAuthenticationOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
            IConfiguration config, IOptionsMonitor<SecurityOptions> securityOptions
            ) : base(options, logger, encoder, clock)
        {
            _config = config;
            _securityOptions = securityOptions.CurrentValue;
        }

        /// <summary>
        /// Builds a fake claims principal and authenticates this claims principal.
        /// The fake claims principal includes a claim representing the user name.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var userNameClaim = _config.GetValue<string>(FakeAuthenticationOptions.ConfigurationKey);
            if (string.IsNullOrEmpty(userNameClaim))
                throw new ArgumentException($"Invalid configuration value for {FakeAuthenticationOptions.ConfigurationKey}");

            var claims = new Claim[] {
                new Claim(_securityOptions.IdpUserNameClaim, userNameClaim),
                new Claim(ClaimTypes.Name, userNameClaim)
            };


            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(FakeAuthenticationHandler)));

            var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }


    }

}
