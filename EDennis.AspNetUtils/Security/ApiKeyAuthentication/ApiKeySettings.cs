
namespace EDennis.AspNetUtils
{
    public class ApiKeySettings
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
        /// Default configuration key for <see cref="ApiKeyAuthenticationOptions"/>
        /// </summary>
        public const string DefaultApiKeyHeaderKey = "X-ApiKey";


        /// <summary>
        /// The default API key value, which should be interpreted as always invalid.
        /// </summary>
        public const string DefaultClaimHeaderPrefix = "X-Claim-";

        /// <summary>
        /// The key to be used when serializing, transmitting
        /// (via HTTP Request Header), deserializing, and 
        /// validating an API Key
        /// </summary>
        public string ApiKeyHeaderKey { get; set; } = DefaultApiKeyHeaderKey;


        /// <summary>
        /// Header prefix for security claims
        /// </summary>
        public string ClaimHeaderPrefix { get; set; } = DefaultClaimHeaderPrefix;

    }
}
