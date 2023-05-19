namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Holds configurations for security
    /// 
    /// Example:
    /// <code>
    /// "Security": {
    ///   "IdpUserNameClaim": "preferred_username",
    ///   "AllowMultipleRoles": false,
    ///   "RefreshInterval": 3600000
    /// }
    /// </code>
    /// </summary>

    public class SecurityOptions
    {

        public const string DefaultConfigKey = "Security";
        public const string FakeRole = "***FAKE***";

        /// <summary>
        /// The claim type from Azure used as the UserName
        /// </summary>
        public string IdpUserNameClaim { get; set; } = "preferred_username";

        public string ApiAccessClaim { get; set; } = "API.Access";


        /// <summary>
        /// whether to allow a single user to have multiple roles
        /// </summary>
        public bool AllowMultipleRoles { get; set; } = false;

        /// <summary>
        /// How long roles should be cached before being refreshed from the Db
        /// </summary>
        public int RefreshInterval { get; set; } = 1000 * 60 * 60; //one hour


    }

}
