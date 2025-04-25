using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// ViewModel for handling user password changes, ensuring validation of both current and new passwords.
    /// </summary>
    public class ChangePasswordViewModel
    {
        /// <summary>
        /// The user's current password, required for verification before allowing a change.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string OldPassword { get; set; }

        /// <summary>
        /// The new password to be set. Must be at least 6 characters long.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters long.")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirmation of the new password. Must match the <see cref="NewPassword"/> field.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation do not match.")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; }
    }
}
