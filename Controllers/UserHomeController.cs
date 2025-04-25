using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Group8_iFINANCE_APP.Data;

namespace Group8_iFINANCE_APP.Controllers
{
    /// <summary>
    /// Displays the user dashboard/home page for non-admin users,
    /// showing summary information such as net worth.
    /// </summary>
    public class UserHomeController : Controller
    {
        private readonly Group8_iFINANCEAPP_DBContext _ctx;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserHomeController"/> class.
        /// </summary>
        /// <param name="ctx">The database context for accessing user data.</param>
        public UserHomeController(Group8_iFINANCEAPP_DBContext ctx)
            => _ctx = ctx;

        /// <summary>
        /// Loads the user's home/index page, including their name and current net worth.
        /// Redirects to login if the user session is not valid.
        /// </summary>
        /// <returns>The Index view with user info, or a redirect to the login page.</returns>
        // GET: /UserHome/Index
        public async Task<IActionResult> Index()
        {
            // Retrieve the current user's ID from session
            var uid = HttpContext.Session.GetInt32("UserId");
            if (uid == null)
                // If no user is signed in, send them to the login page
                return RedirectToAction("Login", "Account");

            // Load the non-admin user along with their master accounts
            var user = await _ctx.NonAdminUsers
                .Include(u => u.MasterAccounts)
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.ID == uid.Value);

            if (user == null)
                // If the user record is missing, treat as unauthenticated
                return RedirectToAction("Login", "Account");

            // Calculate net worth by summing the closing balances of all accounts
            var netWorth = user.MasterAccounts.Sum(ma => ma.ClosingAmount);

            // Pass user name and net worth into the view via ViewBag
            ViewBag.UserName = user.Name;
            ViewBag.NetWorth = netWorth;

            return View();
        }
    }
}
