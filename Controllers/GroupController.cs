using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Group8_iFINANCE_APP.Data;
using Group8_iFINANCE_APP.Models;

namespace Group8_iFINANCE_APP.Controllers
{
    /// <summary>
    /// Manages user-defined account groups, including hierarchical organization (parent/child),
    /// creation, editing, and deletion of groups for the signed-in user.
    /// </summary>
    public class GroupController : Controller
    {
        private readonly Group8_iFINANCEAPP_DBContext _context;

        /// <summary>
        /// Initializes a new instance of <see cref="GroupController"/> with the specified database context.
        /// </summary>
        /// <param name="context">The EF Core database context for persistence operations.</param>
        public GroupController(Group8_iFINANCEAPP_DBContext context)
            => _context = context;

        /// <summary>
        /// Displays all top-level (root) groups along with their nested child groups for the current user.
        /// </summary>
        /// <returns>The view with the hierarchical group structure.</returns>
        // GET: /Group
        public async Task<IActionResult> Index()
        {
            // Verify user is authenticated via session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Load all groups created by this user, including category info
            var all = await _context.Groups
                .Where(g => g.CreatedByUserID == userId.Value)
                .Include(g => g.AccountCategory)
                .ToListAsync();

            // Build lookup for parent → children relationships
            var lookup = all.ToLookup(g => g.parent_ID);
            foreach (var g in all)
                g.Children = lookup[g.ID].ToList();

            // Extract root groups (those with null parent_ID)
            var roots = lookup[null].ToList();
            return View(roots);
        }

        /// <summary>
        /// Renders the creation form, optionally pre-selecting a parent group.
        /// </summary>
        /// <param name="parentId">Optional ID of the parent group for nested creation.</param>
        /// <returns>The create view model with dropdowns for categories and parents.</returns>
        // GET: /Group/Create?parentId=5
        [HttpGet]
        public IActionResult Create(int? parentId)
        {
            // Ensure user is authenticated
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Populate category selector
            ViewBag.Categories = new SelectList(
                _context.AccountCategories.OrderBy(c => c.Name),
                "ID", "Name");

            // Populate parent group selector (excluding none)
            ViewBag.AllParents = new SelectList(
                _context.Groups
                    .Where(g => g.CreatedByUserID == userId.Value)
                    .OrderBy(g => g.Name),
                "ID", "Name");

            ViewBag.ParentId = parentId;
            // Initialize a new Group instance with preset parent_ID
            return View(new Group { parent_ID = parentId });
        }

        /// <summary>
        /// Handles submission of the group creation form, saving a new group record.
        /// </summary>
        /// <param name="form">The bound <see cref="Group"/> model from the form.</param>
        /// <returns>Redirects to index on success or redisplays form on error.</returns>
        // POST: /Group/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Group form)
        {
            // Ensure user is authenticated
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Assign the creating user to the new group
            form.CreatedByUserID = userId.Value;

            // Re-populate dropdowns for redisplay in case of validation errors
            ViewBag.Categories = new SelectList(
                _context.AccountCategories.OrderBy(c => c.Name),
                "ID", "Name", form.AccountCategory_ID);
            ViewBag.AllParents = new SelectList(
                _context.Groups
                    .Where(g => g.CreatedByUserID == userId.Value && g.ID != form.ID)
                    .OrderBy(g => g.Name),
                "ID", "Name", form.parent_ID);
            ViewBag.ParentId = form.parent_ID;

            // Validate model state
            if (!ModelState.IsValid)
                return View(form);

            // Persist the new group entity
            _context.Groups.Add(form);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Loads an existing group for editing, ensuring the user owns it.
        /// </summary>
        /// <param name="id">The ID of the group to edit.</param>
        /// <returns>The edit view or NotFound if unauthorized or missing.</returns>
        // GET: /Group/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Load the group and verify ownership
            var group = await _context.Groups.FindAsync(id);
            if (group == null || group.CreatedByUserID != userId.Value)
                return NotFound();

            // Prepare dropdowns for editing
            ViewBag.Categories = new SelectList(
                _context.AccountCategories.OrderBy(c => c.Name),
                "ID", "Name", group.AccountCategory_ID);
            ViewBag.AllParents = new SelectList(
                _context.Groups
                    .Where(g => g.CreatedByUserID == userId.Value && g.ID != id)
                    .OrderBy(g => g.Name),
                "ID", "Name", group.parent_ID);

            return View(group);
        }

        /// <summary>
        /// Processes edits to an existing group, updating name, category, and parent link.
        /// </summary>
        /// <param name="id">The ID from the URL to confirm against the form data.</param>
        /// <param name="form">The updated <see cref="Group"/> instance.</param>
        /// <returns>Redirects to index on success or redisplays form on validation errors.</returns>
        // POST: /Group/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Group form)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Load and verify existing group
            var group = await _context.Groups.FindAsync(id);
            if (group == null || group.CreatedByUserID != userId.Value)
                return NotFound();

            // Prepare dropdowns again for redisplay
            ViewBag.Categories = new SelectList(
                _context.AccountCategories.OrderBy(c => c.Name),
                "ID", "Name", form.AccountCategory_ID);
            ViewBag.AllParents = new SelectList(
                _context.Groups
                    .Where(g => g.CreatedByUserID == userId.Value && g.ID != id)
                    .OrderBy(g => g.Name),
                "ID", "Name", form.parent_ID);

            if (!ModelState.IsValid)
                return View(form);

            // Apply updates to the tracked entity
            group.Name               = form.Name;
            group.AccountCategory_ID = form.AccountCategory_ID;
            group.parent_ID          = form.parent_ID;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays a confirmation page for deleting a group, including related category and parent info.
        /// </summary>
        /// <param name="id">The ID of the group to delete.</param>
        /// <returns>Confirmation view or NotFound if missing/unauthorized.</returns>
        // GET: /Group/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Load the group with related data
            var group = await _context.Groups
                .Include(g => g.AccountCategory)
                .Include(g => g.ParentGroup)
                .FirstOrDefaultAsync(g => g.ID == id && g.CreatedByUserID == userId.Value);
            if (group == null) return NotFound();
            return View(group);
        }

        /// <summary>
        /// Executes deletion of the specified group if it has no child groups or master accounts.
        /// </summary>
        /// <param name="id">The ID of the group confirmed for deletion.</param>
        /// <returns>Redirects to index or redisplays confirmation with errors if dependencies exist.</returns>
        // POST: /Group/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var group = await _context.Groups.FindAsync(id);
            if (group == null || group.CreatedByUserID != userId.Value)
                return NotFound();

            // Prevent deleting a group that has children or associated accounts
            bool hasChildren = await _context.Groups.AnyAsync(g => g.parent_ID == id);
            bool hasAccounts = await _context.MasterAccounts.AnyAsync(ma => ma.Group_ID == id);
            if (hasChildren || hasAccounts)
            {
                ModelState.AddModelError("", "Cannot delete: sub-groups or accounts exist.");
                return View(group);
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
