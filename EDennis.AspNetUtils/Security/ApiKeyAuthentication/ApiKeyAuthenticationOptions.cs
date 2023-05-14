using Microsoft.AspNetCore.Authentication;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Model class for ApiKey configurations
    /// </summary>
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {

        public const string DefaultConfigKey = "Security:ApiKey";

        /// <summary>
        /// The key to be used when serializing, transmitting
        /// (via HTTP Request Header), deserializing, and 
        /// validating an API Key
        /// </summary>
        public string ApiKeyHeaderKey { get; set; } = ApiKeySettings.DefaultApiKeyHeaderKey;


        /// <summary>
        /// Header prefix for security claims
        /// </summary>
        public string ClaimHeaderPrefix { get; set; } = ApiKeySettings.DefaultClaimHeaderPrefix;

        /// <summary>
        /// The correct/valid value of the ApiKey.  This 
        /// value is a shared secret between the API and
        /// its clients.  In this model, all clients use
        /// the same key; therefore, this model is most
        /// appropriate for internal APIs.
        /// </summary>
        public string ApiKey { get; set; }

    }
}