using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// Represents a non-administrator user in the iFinance system,
    /// including personal details and relationships to accounts and transactions.
    /// </summary>
    public class NonAdminUser
    {
        /// <summary>
        /// Primary key identifier for the non-admin user.
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Full name of the non-admin user.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// Residential or mailing address of the user.
        /// </summary>
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }

        /// <summary>
        /// Contact email address for the user.
        /// </summary>
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        /// <summary>
        /// Foreign key referencing associated login credentials (1:1 relationship).
        /// </summary>
        public int UserPassword_ID { get; set; }

        /// <summary>
        /// Navigation property for the associated <see cref="UserPassword"/> entity.
        /// </summary>
        public UserPassword UserPassword { get; set; }

        /// <summary>
        /// Foreign key linking to the managing administrator (many-to-one relationship).
        /// </summary>
        public int Administrator_ID { get; set; }

        /// <summary>
        /// Navigation property for the assigned <see cref="Administrator"/>.
        /// </summary>
        public Administrator Administrator { get; set; }

        /// <summary>
        /// Collection of master accounts owned by this user.
        /// </summary>
        public ICollection<MasterAccount> MasterAccounts { get; set; }

        /// <summary>
        /// Collection of financial transactions performed by this user.
        /// </summary>
        public ICollection<Transaction> Transactions { get; set; }
    }
}
