using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// ViewModel for user login credentials, capturing the username and password.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// The username used for authentication.
        /// </summary>
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        /// <summary>
        /// The password corresponding to the username.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
