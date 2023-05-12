namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Interface spec for ApiKey options
    /// </summary>
    public interface IApiKeyOptions
    {
        /// <summary>
        /// The key to be used when serializing, transmitting
        /// (via HTTP Request Header), deserializing, and 
        /// validating an API Key
        /// </summary>
        string ApiKey { get; set; }

        /// <summary>
        /// The header key for the ApiKey value
        /// </summary>
        string ApiKeyHeaderKey { get; set; }


        /// <summary>
        /// Header prefix for security claims
        /// </summary>
        string ClaimHeaderPrefix { get; set; }
    }
}