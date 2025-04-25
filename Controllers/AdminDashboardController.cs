using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Group8_iFINANCE_APP.Data;
using Group8_iFINANCE_APP.Models;

namespace Group8_iFINANCE_APP.Controllers
{
    /// <summary>
    /// Provides administrative dashboard functionality, including user and account management.
    /// </summary>
    public class AdminDashboardController : Controller
    {
        private readonly Group8_iFINANCEAPP_DBContext _context;

        /// <summary>
        /// Initializes a new instance of <see cref="AdminDashboardController"/> with the specified database context.
        /// </summary>
        /// <param name="context">The database context for accessing application data.</param>
        public AdminDashboardController(Group8_iFINANCEAPP_DBContext context)
            => _context = context;

        /// <summary>
        /// Displays the main dashboard view with summary statistics.
        /// </summary>
        /// <returns>The dashboard index view.</returns>
        // GET: /AdminDashboard/Index
        public async Task<IActionResult> Index()
        {
            // Count total administrators and non-admin users for display
            ViewBag.TotalAdmins    = await _context.Administrators.CountAsync();
            ViewBag.TotalNonAdmins = await _context.NonAdminUsers.CountAsync();
            return View();
        }

        /// <summary>
        /// Shows a list of all administrator accounts.
        /// </summary>
        /// <returns>A view containing all administrators.</returns>
        // GET: /AdminDashboard/ListAdmins
        public async Task<IActionResult> ListAdmins()
        {
            // Retrieve administrators without tracking for read-only performance
            var admins = await _context.Administrators
                                       .AsNoTracking()
                                       .ToListAsync();
            return View(admins);
        }

        /// <summary>
        /// Shows a list of all non-admin user accounts, including their assigned administrator.
        /// </summary>
        /// <returns>A view containing all non-admin users.</returns>
        // GET: /AdminDashboard/ListNonAdmins
        public async Task<IActionResult> ListNonAdmins()
        {
            // Include related Administrator for each non-admin user
            var users = await _context.NonAdminUsers
                                      .Include(u => u.Administrator)
                                      .AsNoTracking()
                                      .ToListAsync();
            return View(users);
        }

        /// <summary>
        /// Displays the form for creating a new user (admin or non-admin).
        /// </summary>
        /// <returns>The user creation view.</returns>
        // GET: /AdminDashboard/AddUser
        [HttpGet]
        public IActionResult AddUser()
        {
            // Populate dropdown with account categories sorted by name
            ViewBag.InitialCategories = new SelectList(
                _context.AccountCategories.OrderBy(c => c.Name).ToList(),
                "ID", "Name"
            );
            return View();
        }

        /// <summary>
        /// Handles form submission for creating a new admin or non-admin user.
        /// </summary>
        /// <param name="model">The view model containing user data and role selection.</param>
        /// <returns>Redirects back to the dashboard index on success, or redisplays the form on error.</returns>
        // POST: /AdminDashboard/AddUser
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(CreateUserViewModel model)
        {
            // Re-populate dropdown list in case the view must be redisplayed
            ViewBag.InitialCategories = new SelectList(
                _context.AccountCategories.OrderBy(c => c.Name).ToList(),
                "ID", "Name"
            );

            // Remove irrelevant fields based on selected role
            if (model.Role == "Admin")
            {
                ModelState.Remove(nameof(model.Address));
                ModelState.Remove(nameof(model.Email));
                ModelState.Remove(nameof(model.InitialCategoryID));
                ModelState.Remove(nameof(model.OpeningAmount));
            }
            else // NonAdmin
            {
                ModelState.Remove(nameof(model.PasswordExpiryTime));
                ModelState.Remove(nameof(model.UserAccountExpiryDate));
                ModelState.Remove(nameof(model.DateHired));
            }

            // If validation fails, return to the form view
            if (!ModelState.IsValid)
                return View(model);

            // 1) Create and save credential record
            var up = new UserPassword
            {
                UserName              = model.Username,
                EncryptedPassword     = model.Password,
                PasswordExpiryTime    = model.PasswordExpiryTime,
                UserAccountExpiryDate = model.UserAccountExpiryDate
            };
            _context.UserPasswords.Add(up);
            await _context.SaveChangesAsync();

            if (model.Role == "Admin")
            {
                // 2a) Create administrator record
                var admin = new Administrator
                {
                    Name            = model.Name,
                    DateHired       = model.DateHired ?? DateTime.Now,
                    DateFinished    = null,
                    UserPassword_ID = up.ID
                };
                _context.Administrators.Add(admin);
                await _context.SaveChangesAsync();
            }
            else
            {
                // 2b) Create non-admin user record
                var sessionAdminId = HttpContext.Session.GetInt32("UserId") ?? 0;
                if (sessionAdminId <= 0)
                {
                    // Fallback: assign first admin if session is unavailable
                    sessionAdminId = (await _context.Administrators
                        .OrderBy(a => a.ID)
                        .FirstOrDefaultAsync())?.ID ?? 0;
                }

                var nonAdmin = new NonAdminUser
                {
                    Name             = model.Name,
                    Address          = model.Address,
                    Email            = model.Email,
                    UserPassword_ID  = up.ID,
                    Administrator_ID = sessionAdminId
                };
                _context.NonAdminUsers.Add(nonAdmin);
                await _context.SaveChangesAsync();

                // 3) Optionally seed initial account group and opening balance
                if (model.InitialCategoryID.HasValue && model.OpeningAmount.HasValue)
                {
                    // a) Create personal account group
                    var category = await _context.AccountCategories
                                                 .FindAsync(model.InitialCategoryID.Value);
                    var grp = new Group
                    {
                        Name               = $"{nonAdmin.Name}’s {category.Name} Group",
                        AccountCategory_ID = model.InitialCategoryID.Value,
                        parent_ID          = null,
                        CreatedByUserID    = nonAdmin.ID
                    };
                    _context.Groups.Add(grp);
                    await _context.SaveChangesAsync();

                    // b) Create opening balance master account
                    var acct = new MasterAccount
                    {
                        Name             = "Opening Balance",
                        OpeningAmount    = (double)model.OpeningAmount.Value,
                        ClosingAmount    = (double)model.OpeningAmount.Value,
                        Group_ID         = grp.ID,
                        NonAdminUser_ID  = nonAdmin.ID
                    };
                    _context.MasterAccounts.Add(acct);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the edit form for an administrator.
        /// </summary>
        /// <param name="id">The administrator ID to edit.</param>
        /// <returns>The admin edit view or 404 if not found.</returns>
        // GET: /AdminDashboard/EditAdmin/5
        [HttpGet]
        public async Task<IActionResult> EditAdmin(int id)
        {
            var admin = await _context.Administrators.FindAsync(id);
            if (admin == null) return NotFound();
            return View(admin);
        }

        /// <summary>
        /// Processes edit requests for an administrator's details.
        /// </summary>
        /// <param name="id">The ID in the URL to verify against the form.</param>
        /// <param name="form">The bound administrator form data.</param>
        /// <returns>Redirects to the admin list on success or redisplays the form on error.</returns>
        // POST: /AdminDashboard/EditAdmin/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdmin(int id,
            [Bind("ID,Name,DateHired,DateFinished")] Administrator form)
        {
            if (id != form.ID) return BadRequest();
            // Exclude navigation properties from model validation
            ModelState.Remove(nameof(Administrator.UserPassword));
            if (!ModelState.IsValid) return View(form);

            var admin = await _context.Administrators.FindAsync(id);
            if (admin == null) return NotFound();

            // Update relevant fields and save
            admin.Name         = form.Name;
            admin.DateHired    = form.DateHired;
            admin.DateFinished = form.DateFinished;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListAdmins));
        }

        /// <summary>
        /// Displays a confirmation page for deleting an administrator.
        /// </summary>
        /// <param name="id">The administrator ID to delete.</param>
        /// <returns>The delete confirmation view or 404 if not found.</returns>
        // GET: /AdminDashboard/DeleteAdmin/5
        [HttpGet]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _context.Administrators.FindAsync(id);
            if (admin == null) return NotFound();
            return View(admin);
        }

        /// <summary>
        /// Processes administrator deletion after confirmation, ensuring no dependent users exist.
        /// </summary>
        /// <param name="id">The administrator ID confirmed for deletion.</param>
        /// <returns>Redirects to the admin list or redisplays with errors if dependencies exist.</returns>
        // POST: /AdminDashboard/DeleteAdmin/5
        [HttpPost, ActionName("DeleteAdmin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAdminConfirmed(int id)
        {
            // Prevent deletion if this admin has assigned non-admin users
            if (await _context.NonAdminUsers.AnyAsync(u => u.Administrator_ID == id))
            {
                var stuck = await _context.Administrators.FindAsync(id);
                ModelState.AddModelError("", 
                  "Cannot delete: there are non-admin users assigned to this admin.");
                return View(stuck);
            }

            var admin = await _context.Administrators.FindAsync(id);
            _context.Administrators.Remove(admin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListAdmins));
        }

        /// <summary>
        /// Displays the edit form for a non-admin user, including administrator assignment.
        /// </summary>
        /// <param name="id">The non-admin user ID to edit.</param>
        /// <returns>The non-admin edit view or 404 if not found.</returns>
        // GET: /AdminDashboard/EditNonAdmin/5
        [HttpGet]
        public async Task<IActionResult> EditNonAdmin(int id)
        {
            var user = await _context.NonAdminUsers.FindAsync(id);
            if (user == null) return NotFound();

            // Populate administrator dropdown with the current selection
            ViewBag.Administrators = new SelectList(
                _context.Administrators, "ID", "Name", user.Administrator_ID);
            return View(user);
        }

        /// <summary>
        /// Processes edit requests for a non-admin user's details.
        /// </summary>
        /// <param name="id">The ID in the URL to verify against the form.</param>
        /// <param name="form">The bound non-admin user form data.</param>
        /// <returns>Redirects to the non-admin list on success or redisplays the form on error.</returns>
        // POST: /AdminDashboard/EditNonAdmin/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNonAdmin(int id,
            [Bind("ID,Name,Address,Email,Administrator_ID")] NonAdminUser form)
        {
            if (id != form.ID) return BadRequest();
            // Exclude navigation and related properties from validation
            ModelState.Remove(nameof(NonAdminUser.UserPassword));
            ModelState.Remove(nameof(NonAdminUser.Transactions));
            ModelState.Remove(nameof(NonAdminUser.MasterAccounts));
            if (!ModelState.IsValid)
            {
                ViewBag.Administrators = new SelectList(
                    _context.Administrators, "ID", "Name", form.Administrator_ID);
                return View(form);
            }

            var user = await _context.NonAdminUsers.FindAsync(id);
            if (user == null) return NotFound();

            // Update relevant fields and save
            user.Name             = form.Name;
            user.Address          = form.Address;
            user.Email            = form.Email;
            user.Administrator_ID = form.Administrator_ID;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListNonAdmins));
        }

        /// <summary>
        /// Displays a confirmation page for deleting a non-admin user, including their assigned admin.
        /// </summary>
        /// <param name="id">The non-admin user ID to delete.</param>
        /// <returns>The delete confirmation view or 404 if not found.</returns>
        // GET: /AdminDashboard/DeleteNonAdmin/5
        [HttpGet]
        public async Task<IActionResult> DeleteNonAdmin(int id)
        {
            var user = await _context.NonAdminUsers
                .Include(u => u.Administrator)
                .FirstOrDefaultAsync(u => u.ID == id);
            if (user == null) return NotFound();
            return View(user);
        }

        /// <summary>
        /// Processes deletion of a non-admin user after confirmation, ensuring no transactions exist.
        /// </summary>
        /// <param name="id">The non-admin user ID confirmed for deletion.</param>
        /// <returns>Redirects to the non-admin list or redisplays with errors if dependencies exist.</returns>
        // POST: /AdminDashboard/DeleteNonAdmin/5
        [HttpPost, ActionName("DeleteNonAdmin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNonAdminConfirmed(int id)
        {
            // Prevent deletion if the user has existing financial transactions
            if (await _context.Transactions.AnyAsync(t => t.NonAdminUser_ID == id))
            {
                var stuck = await _context.NonAdminUsers
                    .Include(u => u.Administrator)
                    .FirstOrDefaultAsync(u => u.ID == id);
                ModelState.AddModelError("",
                  "Cannot delete: this user has existing transactions.");
                return View(stuck);
            }

            // Remove the user record and save changes
            var user = await _context.NonAdminUsers.FindAsync(id);
            _context.NonAdminUsers.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListNonAdmins));
        }
    }
}
