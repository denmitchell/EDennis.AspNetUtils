using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Authentication handler for ApiKey authentication.  ApiKey authentication
    /// works by validating transmission of a shared key.  Along with the shared
    /// key, the transmission includes a collection of security claims.  This 
    /// form of authentication is appropriate for internal APIs, where sharing
    /// the same security key is not a risk.
    /// </summary>
    public class ApiKeyAuthenticationHandler
        : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {

        private readonly ApiKeyAuthenticationOptions _settings;

        /// <summary>
        /// Constructs a new ApiKeyAuthenticationHandler instance
        /// using the provided arguments
        /// </summary>
        /// <param name="options">ApiKey settings, including the valid/correct value of the key</param>
        /// <param name="logger">Logger for logging information or exceptions</param>
        /// <param name="encoder">Used for creating safe URLs</param>
        /// <param name="clock">Allows injection of real or mocked time</param>
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _settings = options.CurrentValue;
        }

        /// <summary>
        /// Implements the logic for authenticating a client and creating
        /// a ClaimsPrincipal.  In this case, an ApiKeyToken transmitted as 
        /// a string through a header is deserialized, validated, and used 
        /// to create a ClaimsPricipal with the claims included in token.
        /// </summary>
        /// <returns>a failure or success result</returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            //try to get the token from the header
            if (!Context.Request.Headers.TryGetValue(_settings.ApiKeyHeaderKey, out StringValues apiKeyValues))
            {
                return await Task.FromResult(AuthenticateResult.NoResult());
            }

            var apiKey = apiKeyValues.ToString();


            //fail if the token's ApiKey claim value doesn't match the expected/correct value
            if (apiKey != _settings.ApiKey)
            {
                await HandleErrorAsync(Context);
                return await Task.FromResult(AuthenticateResult.Fail($"Invalid ApiKey."));
            }


            IEnumerable<Claim> claims = null;

            //try to get the token from the header
            var claimsHeaders = Context.Request.Headers
                .Where(c => c.Key.StartsWith(_settings.ClaimHeaderPrefix))
                .SelectMany(c =>
                {
                    var key = c.Key[_settings.ClaimHeaderPrefix.Length..];
                    return c.Value.ToArray().Select(v => new Claim(key, v));
                });


            //create a new ClaimsPrincipal and an authentication ticket.
            var claimsIdentity = new ClaimsIdentity(claims, nameof(ApiKeyAuthenticationHandler));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

            //return success
            return await Task.FromResult(AuthenticateResult.Success(ticket));


        }

        /// <summary>
        /// Log the authentication as an error
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task HandleErrorAsync(HttpContext context)
        {
            await Task.Run(() =>
            {
                var url = context.Request.Path.Value.Split('?')[0];
                var exception = new ArgumentException($"Attempt to access @{url} without ApiKey");
                Logger.LogError(exception, "Attempt to access API @{Url} without ApiKey", url);
            });
        }

    }
}

