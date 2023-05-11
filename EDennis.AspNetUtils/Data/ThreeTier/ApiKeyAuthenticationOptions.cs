using Microsoft.AspNetCore.Authentication;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Model class for ApiKey configurations
    /// </summary>
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {

        /// <summary>
        /// Default configuration key for <see cref="ApiKeyAuthenticationOptions"/>
        /// </summary>
        public const string DefaultConfigKey = "Security:ApiKey";


        /// <summary>
        /// The default API key value, which should be interpreted as always invalid.
        /// </summary>
        public const string DefaultApiKeyValue = "00000000-0000-0000-0000-000000000000";

        /// <summary>
        /// The key to be used when serializing, transmitting
        /// (via HTTP Request Header), deserializing, and 
        /// validating an API Key
        /// </summary>
        public string ApiKeyHeaderKey { get; set; } = "X-ApiKey";


        /// <summary>
        /// The Claims Principal Name
        /// </summary>
        public string ClaimHeaderPrefix { get; set; } = "X-Claim-";

        /// <summary>
        /// The correct/valid value of the ApiKey.  This 
        /// value is a shared secret between the API and
        /// its clients.  In this model, all clients use
        /// the same key; therefore, this model is most
        /// appropriate for internal APIs.
        /// </summary>
        public string ApiKeyValue { get; set; }

    }
}