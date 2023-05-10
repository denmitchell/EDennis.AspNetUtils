namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Model class representing an application role.
    /// The backing table holds all roles for the application
    /// </summary>
    public partial class AppRole : EntityBase
    {

        /// <summary>
        /// The text label for the role
        /// </summary>
        public string RoleName { get; set; }

    }
}