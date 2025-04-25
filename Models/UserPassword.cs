using System;
using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// Stores authentication credentials and related expiry settings for both administrators and users.
    /// </summary>
    public class UserPassword
    {
        /// <summary>
        /// Primary key identifier for the credential record.
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// The unique username for login.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
        public string UserName { get; set; }

        /// <summary>
        /// The stored, encrypted representation of the user's password.
        /// </summary>
        [Required]
        public string EncryptedPassword { get; set; }

        /// <summary>
        /// Optional number of days after which the password must be changed.
        /// </summary>
        [Display(Name = "Password expiry (days)")]
        public int? PasswordExpiryTime { get; set; }

        /// <summary>
        /// Optional date when the user account expires and can no longer be used.
        /// </summary>
        [Display(Name = "Account expiry date")]
        [DataType(DataType.Date)]
        public DateTime? UserAccountExpiryDate { get; set; }

        /// <summary>
        /// Navigation property back to the associated administrator (1:1 relationship).
        /// </summary>
        public Administrator Administrator { get; set; }

        /// <summary>
        /// Navigation property back to the associated non-admin user (1:1 relationship).
        /// </summary>
        public NonAdminUser NonAdminUser { get; set; }
    }
}
