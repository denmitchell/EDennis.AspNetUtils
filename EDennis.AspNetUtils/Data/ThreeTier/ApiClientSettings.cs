
namespace EDennis.AspNetUtils
{
    public class ApiClientSettingsDictionary : Dictionary<string, ApiClientSettings> { }
    public class ApiClientSettings : IApiKeyOptions
    {

        /// <summary>
        /// The key to be used when serializing, transmitting
        /// (via HTTP Request Header), deserializing, and 
        /// validating an API Key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The header key for the ApiKey value
        /// </summary>
        public string ApiKeyHeaderKey { get; set; } = "X-ApiKey";


        /// <summary>
        /// Header prefix for security claims
        /// </summary>
        public string ClaimHeaderPrefix { get; set; } = "X-Claim-";


        /// <summary>
        /// Base address for the ApiClient
        /// </summary>
        public string BaseAddress { get; set; }

    }
}
