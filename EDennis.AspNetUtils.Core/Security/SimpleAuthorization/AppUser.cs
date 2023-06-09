using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Model class representing an application user.
    /// The backing table holds all users for the application
    /// </summary>
    public partial class AppUser : EntityBase
    {
        /// <summary>
        /// The user name of the application.  It is generally advised
        /// to use email address here.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// The user's role 
        /// </summary>
        public string Role { get; set; }

        [NotMapped]
        public string Scope { get; set; }

        [NotMapped]
        public bool IsFake { get; set; }

    }
}