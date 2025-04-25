using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Group8_iFINANCE_APP.Data;
using Group8_iFINANCE_APP.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Group8_iFINANCE_APP.Controllers
{
    /// <summary>
    /// Manages the Chart of Accounts for the current user, including viewing, creating, and deleting accounts.
    /// </summary>
    public class ChartOfAccountsController : Controller
    {
        private readonly Group8_iFINANCEAPP_DBContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartOfAccountsController"/> class.
        /// </summary>
        /// <param name="ctx">The database context used to access account and group data.</param>
        public ChartOfAccountsController(Group8_iFINANCEAPP_DBContext ctx)
            => _context = ctx;

        /// <summary>
        /// Displays all master accounts belonging to the currently signed-in user.
        /// </summary>
        /// <returns>The index view with a list of master accounts.</returns>
        // GET: /ChartOfAccounts
        public async Task<IActionResult> Index()
        {
            // Retrieve the current user's ID from session (default to 0 if missing)
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Load all MasterAccounts where the parent Group was created by this user
            var accounts = await _context.MasterAccounts
                .Include(ma => ma.Group)
                .Where(ma => ma.Group.CreatedByUserID == userId)
                .ToListAsync();

            return View(accounts);
        }

        /// <summary>
        /// Renders the form to create a new master account, including dropdowns for categories and existing groups.
        /// </summary>
        /// <returns>The create view model populated with select lists.</returns>
        // GET: /ChartOfAccounts/Create
        [HttpGet]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Prepare view model with category and existing group selectors
            var vm = new MasterAccountCreateViewModel
            {
                Categories = new SelectList(
                    _context.AccountCategories.OrderBy(c => c.Name),
                    "ID", "Name"),
                ExistingGroups = new SelectList(
                    _context.Groups
                        .Where(g => g.CreatedByUserID == userId)
                        .OrderBy(g => g.Name),
                    "ID", "Name")
            };

            return View(vm);
        }

        /// <summary>
        /// Handles the submission of a new master account, creating either a new group or using an existing one.
        /// </summary>
        /// <param name="vm">The view model containing account details and group selection.</param>
        /// <returns>Redirects to index on success or redisplays the form on validation errors.</returns>
        // POST: /ChartOfAccounts/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MasterAccountCreateViewModel vm)
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Re-populate dropdowns before validation in case of errors
            vm.Categories = new SelectList(
                _context.AccountCategories.OrderBy(c => c.Name),
                "ID", "Name", vm.CategoryID);
            vm.ExistingGroups = new SelectList(
                _context.Groups
                    .Where(g => g.CreatedByUserID == userId)
                    .OrderBy(g => g.Name),
                "ID", "Name", vm.GroupID);

            if (!ModelState.IsValid)
                return View(vm);

            int groupId;
            // 1) Determine whether to create a new group
            if (!string.IsNullOrWhiteSpace(vm.NewGroupName))
            {
                var grp = new Group
                {
                    Name               = vm.NewGroupName,
                    AccountCategory_ID = vm.CategoryID,
                    parent_ID          = vm.ParentGroupID,
                    CreatedByUserID    = userId
                };
                _context.Groups.Add(grp);
                await _context.SaveChangesAsync();
                groupId = grp.ID;
            }
            else if (vm.GroupID.HasValue)
            {
                // Use selected existing group
                groupId = vm.GroupID.Value;
            }
            else
            {
                // Neither new nor existing group specified, add validation error
                ModelState.AddModelError(
                    nameof(vm.GroupID),
                    "Please either pick an existing group or type a new name.");
                return View(vm);
            }

            // 2) Create and save the master account in the chosen group
            var acct = new MasterAccount
            {
                Name            = vm.Name,
                OpeningAmount   = vm.OpeningAmount,
                ClosingAmount   = vm.OpeningAmount,
                Group_ID        = groupId,
                NonAdminUser_ID = userId  // Link account to current user
            };
            _context.MasterAccounts.Add(acct);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the confirmation view for deleting a master account.
        /// </summary>
        /// <param name="id">The ID of the master account to delete.</param>
        /// <returns>The delete view or NotFound if the account doesn't exist.</returns>
        // GET: /ChartOfAccounts/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var acct = await _context.MasterAccounts
                .Include(ma => ma.Group)
                .FirstOrDefaultAsync(ma => ma.ID == id);

            if (acct == null)
                return NotFound();

            return View(acct);
        }

        /// <summary>
        /// Performs deletion of the specified master account after confirmation.
        /// </summary>
        /// <param name="id">The ID of the master account to delete.</param>
        /// <returns>Redirects to index after deletion.</returns>
        // POST: /ChartOfAccounts/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var acct = await _context.MasterAccounts.FindAsync(id);
            if (acct != null)
            {
                _context.MasterAccounts.Remove(acct);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
