using System;
using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// ViewModel used for creating new users in the admin dashboard,
    /// capturing both admin-specific and non-admin-specific fields.
    /// </summary>
    public class CreateUserViewModel
    {
        /// <summary>
        /// Role selection for the new user ("Admin" or "NonAdmin").
        /// </summary>
        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        /// <summary>
        /// Full name of the new user.
        /// </summary>
        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        // -- Credentials --

        /// <summary>
        /// Username for login credentials.
        /// </summary>
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        /// <summary>
        /// Initial password for the new user.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        // Admin-only fields

        /// <summary>
        /// Number of days until the password expires (admins only).
        /// </summary>
        [Display(Name = "Password Expiry (days)")]
        public int? PasswordExpiryTime { get; set; }

        /// <summary>
        /// Date when the user account will expire (admins only).
        /// </summary>
        [Display(Name = "Account Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime? UserAccountExpiryDate { get; set; }

        /// <summary>
        /// The date the administrator was hired (admins only).
        /// </summary>
        [Display(Name = "Date Hired")]
        [DataType(DataType.Date)]
        public DateTime? DateHired { get; set; }

        // Non-admin-specific fields

        /// <summary>
        /// Address of the non-admin user.
        /// </summary>
        [Display(Name = "Address")]
        public string Address { get; set; }

        /// <summary>
        /// Email address of the non-admin user.
        /// </summary>
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        // Initial setup for non-admin seed data

        /// <summary>
        /// Identifier for the initial account category when seeding the first group.
        /// </summary>
        [Display(Name = "Initial Category")]
        public int? InitialCategoryID { get; set; }

        /// <summary>
        /// Opening amount to seed into the user's first account.
        /// </summary>
        [Display(Name = "Opening Amount")]
        [Range(0, double.MaxValue)]
        public decimal? OpeningAmount { get; set; }
    }
}
