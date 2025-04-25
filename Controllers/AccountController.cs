using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Group8_iFINANCE_APP.Data;
using Group8_iFINANCE_APP.Models;
using System.Threading.Tasks;

namespace Group8_iFINANCE_APP.Controllers
{
    /// <summary>
    /// Handles user account operations such as login, logout, and password management.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly Group8_iFINANCEAPP_DBContext _db;

        /// <summary>
        /// Initializes a new instance of <see cref="AccountController"/> with the specified database context.
        /// </summary>
        /// <param name="db">The database context for accessing user-related data.</param>
        public AccountController(Group8_iFINANCEAPP_DBContext db)
            => _db = db;

        /// <summary>
        /// Displays the login page.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
            => View();

        /// <summary>
        /// Processes login submissions and establishes a session for authenticated users.
        /// </summary>
        /// <param name="model">The login credentials provided by the user.</param>
        /// <returns>
        /// Redirects to the appropriate dashboard on success or redisplays the login view on failure.
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Ensure the submitted form data satisfies validation rules
            if (!ModelState.IsValid)
                return View(model);

            // Retrieve the credential record matching the provided username
            var cred = await _db.UserPasswords
                                .FirstOrDefaultAsync(u => u.UserName == model.Username);
            if (cred == null)
            {
                // Username does not exist in the system
                ModelState.AddModelError(nameof(model.Username), "Username not found.");
                return View(model);
            }

            // Compare the stored encrypted password with the one supplied by the user
            if (cred.EncryptedPassword != model.Password)
            {
                // Provided password does not match the record
                ModelState.AddModelError(nameof(model.Password), "Incorrect password.");
                return View(model);
            }

            // Check if the authenticated user is an administrator
            var admin = await _db.Administrators
                                 .FirstOrDefaultAsync(a => a.UserPassword_ID == cred.ID);
            if (admin != null)
            {
                // Set session variables for an admin user
                HttpContext.Session.SetInt32("UserType", 1);
                HttpContext.Session.SetInt32("UserId",   admin.ID);
                HttpContext.Session.SetString("Username", cred.UserName);
                return RedirectToAction("Index", "AdminDashboard");
            }

            // Check if the authenticated user is a non-admin user
            var user = await _db.NonAdminUsers
                                .FirstOrDefaultAsync(u => u.UserPassword_ID == cred.ID);
            if (user != null)
            {
                // Set session variables for a non-admin user
                HttpContext.Session.SetInt32("UserType", 2);
                HttpContext.Session.SetInt32("UserId",   user.ID);
                HttpContext.Session.SetString("Username", cred.UserName);
                return RedirectToAction("Index", "UserHome");
            }

            // If the user has no associated role, display an error
            ModelState.AddModelError("", "No role assigned for that account.");
            return View(model);
        }

        /// <summary>
        /// Logs out the current user by clearing their session.
        /// </summary>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Displays the change password page for authenticated users.
        /// </summary>
        [HttpGet]
        public IActionResult ChangePassword()
        {
            // Redirect unauthenticated users to the login page
            if (HttpContext.Session.GetInt32("UserType") == null)
                return RedirectToAction("Login");

            return View(new ChangePasswordViewModel());
        }

        /// <summary>
        /// Handles password update requests by verifying the old password and saving the new one.
        /// </summary>
        /// <param name="m">The view model containing current and new password data.</param>
        /// <returns>
        /// Redirects to the login page on success or redisplays the form on failure.
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel m)
        {
            // Ensure the form submission meets validation requirements
            if (!ModelState.IsValid)
                return View(m);

            // Retrieve the user's credential record based on the active session
            var cred = await _db.UserPasswords
                                .FirstOrDefaultAsync(u => u.UserName == HttpContext.Session.GetString("Username"));

            if (cred == null)
            {
                // If no credential record is found, treat as unauthenticated
                return RedirectToAction("Login");
            }

            // Confirm that the old password matches the stored value
            if (cred.EncryptedPassword != m.OldPassword)
            {
                ModelState.AddModelError(nameof(m.OldPassword), "Current password is incorrect.");
                return View(m);
            }

            // Update the record with the new password and persist changes
            cred.EncryptedPassword = m.NewPassword;
            await _db.SaveChangesAsync();

            // Notify the user of success and redirect to login
            TempData["Message"] = "Your password has been updated.";
            return RedirectToAction("Login", "Account");
        }
    }
}
