using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// DelegatingHandler for <see cref="HttpRequestMessage"/>s sent from an
    /// <see cref="HttpClient"/> to an application configured to handle
    /// ApiKey authentication using an <see cref="ApiKeyAuthenticationHandler"/>.
    /// This handler can be manually added to an HttpClient's set of 
    /// handlers.  Alternatively, <see cref="ApiClientExtensions.AddApiClients(Microsoft.Extensions.DependencyInjection.IServiceCollection, Microsoft.Extensions.Configuration.IConfiguration, Model.SecurityMode, string, string)"/> will automatically add this 
    /// handler when relevant, based upon Configuration.
    /// </summary>
    /// <seealso cref="DelegateTokenMessageHandler"/>
    /// <seealso cref="ClientCredentialsMessageHandler"/>
    public class ApiKeyMessageHandler : DelegatingHandler
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApiClientSettingsDictionary _dict;
        private readonly ApiKeySettings _apiKeySettings;
        private readonly SecurityOptions _securityOptions;

        /// <summary>
        /// Constructs a new ApiKeyMessageHandler using the provided
        /// HttpContextAccessor and ApiClientSettingsDictionary
        /// </summary>
        /// <param name="httpContextAccessor">Provides access to the HttpContext</param>
        /// <param name="dict">Dictionary of all settings for API Clients</param>
        public ApiKeyMessageHandler(IHttpContextAccessor httpContextAccessor,
            IOptionsMonitor<ApiClientSettingsDictionary> dict,
            IOptionsMonitor<ApiKeySettings> apiKeySettings,
            IOptionsMonitor<SecurityOptions> securityOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _dict = dict.CurrentValue;
            _securityOptions = securityOptions.CurrentValue;
            _apiKeySettings = apiKeySettings.CurrentValue;
        }

        /// <summary>
        /// Sends an HTTP request.
        /// </summary>
        /// <param name="request">The HTTP request message to send</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The Http response message</returns>
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddHeader(request);
            return base.Send(request, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request asynchronously.
        /// </summary>
        /// <param name="request">The HTTP request message to send</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The Http response message</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddHeader(request);
            return await base.SendAsync(request, cancellationToken);
        }


        /// <summary>
        /// Constructs X-ApiKey and X-Claim- headers and adds them to the message.
        /// </summary>
        /// <param name="request"></param>
        private void AddHeader(HttpRequestMessage request)
        {
            //find the appropriate Api in the Apis configuration,
            //using the BaseAddress property.
            var baseAddress = request.RequestUri.GetLeftPart(UriPartial.Path);
            var entry = _dict.FirstOrDefault(d => baseAddress
                .StartsWith(d.Value.BaseAddress));

            if (entry.Key == default)
                throw new ArgumentException($"No Api configuration matches {baseAddress}");


            var clientSettings = entry.Value;
            var apiKey = clientSettings.Properties["ApiKey"];

            //get all user claims
            var claims = _httpContextAccessor.HttpContext.User?.Claims;
            var userName = claims.FirstOrDefault(c => c.Type == _securityOptions.IdpUserNameClaim);
            if (apiKey != null)
                request.Headers.Add(_apiKeySettings.ApiKeyHeaderKey, apiKey);
            if (userName != null)
                request.Headers.Add($"{_apiKeySettings.ClaimHeaderPrefix}c.Type", userName.Value);

        }
    }
}
