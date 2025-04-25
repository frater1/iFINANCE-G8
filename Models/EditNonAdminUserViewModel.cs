using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// ViewModel for editing non-admin user details in the admin dashboard,
    /// ensuring required fields and proper validation attributes.
    /// </summary>
    public class EditNonAdminUserViewModel
    {
        /// <summary>
        /// Unique identifier of the non-admin user.
        /// </summary>
        [Required]
        public int ID { get; set; }

        /// <summary>
        /// Full name of the non-admin user.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Residential or mailing address of the user.
        /// </summary>
        [Required]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Contact email for the non-admin user.
        /// </summary>
        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Identifier for the administrator assigned to manage this user.
        /// </summary>
        [Required]
        [Display(Name = "Administrator")]
        public int AdministratorID { get; set; }
    }
}
