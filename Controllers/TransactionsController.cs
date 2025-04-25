using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Group8_iFINANCE_APP.Data;
using Group8_iFINANCE_APP.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Group8_iFINANCE_APP.Controllers
{
    /// <summary>
    /// Manages financial transactions, including listing past transactions and creating new ones.
    /// </summary>
    public class TransactionsController : Controller
    {
        private readonly Group8_iFINANCEAPP_DBContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionsController"/> class with the specified database context.
        /// </summary>
        /// <param name="ctx">The database context for accessing transaction data.</param>
        public TransactionsController(Group8_iFINANCEAPP_DBContext ctx)
            => _context = ctx;

        /// <summary>
        /// Displays a list of all transactions for the current user, ordered by date descending.
        /// </summary>
        /// <returns>The index view with a list of <see cref="TransactionIndexViewModel"/> items.</returns>
        // GET: /Transactions
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            
            // Load all transactions for this user and include their line items
            var txns = await _context.Transactions
                .Where(t => t.NonAdminUser_ID == userId)
                .Include(t => t.TransactionLines)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            // Map to view model, summing debited amounts for the transaction total
            var vm = txns.Select(t => new TransactionIndexViewModel {
                ID          = t.ID,
                Date        = t.Date,
                Description = t.Description,
                Amount      = t.TransactionLines.Sum(l => l.DebitedAmount)
            }).ToList();

            return View(vm);
        }

        /// <summary>
        /// Prepares and renders the form for creating a new transaction.
        /// </summary>
        /// <returns>The create view with a populated accounts dropdown.</returns>
        // GET: /Transactions/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateAccountsDropdown();
            return View(new TransactionCreateViewModel());
        }

        /// <summary>
        /// Processes the submission of a new transaction, including validation, creating transaction and line entries, and updating balances.
        /// </summary>
        /// <param name="dto">The view model containing transaction details (accounts, amount, comments).</param>
        /// <returns>Redirects to Index on success or redisplays the form with validation errors.</returns>
        // POST: /Transactions/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionCreateViewModel dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            // Ensure dropdowns remain populated on redisplay
            await PopulateAccountsDropdown();

            // Load all user-owned accounts into dictionary for quick lookups
            var accounts = await _context.MasterAccounts
                .Where(ma => ma.Group.CreatedByUserID == userId)
                .ToDictionaryAsync(ma => ma.ID);

            // Validate selected account IDs and transaction amount
            if (!accounts.ContainsKey(dto.DebitAccountId))
                ModelState.AddModelError(nameof(dto.DebitAccountId), "Invalid source account.");
            if (!accounts.ContainsKey(dto.CreditAccountId))
                ModelState.AddModelError(nameof(dto.CreditAccountId), "Invalid destination account.");
            if (dto.DebitAccountId == dto.CreditAccountId)
                ModelState.AddModelError("", "Cannot transfer to the same account.");
            if (dto.Amount <= 0)
                ModelState.AddModelError(nameof(dto.Amount), "Amount must be positive.");

            // Ensure sufficient funds for debit account
            if (accounts.ContainsKey(dto.DebitAccountId) 
             && accounts[dto.DebitAccountId].ClosingAmount < dto.Amount)
                ModelState.AddModelError(nameof(dto.Amount), "Insufficient funds.");

            // If any validation failed, return the form view
            if (!ModelState.IsValid)
                return View(dto);

            // Retrieve account entities for debit and credit
            var from = accounts[dto.DebitAccountId];
            var to   = accounts[dto.CreditAccountId];

            // 1) Create the transaction header record
            var txn = new Transaction {
                Date            = dto.Date,
                Description     = dto.Description,
                NonAdminUser_ID = userId
            };
            _context.Transactions.Add(txn);
            await _context.SaveChangesAsync();

            // 2) Create the debit and credit line items
            var debitLine = new TransactionLine {
                Transaction_ID     = txn.ID,
                MasterAccounts_ID  = from.ID,
                MasterAccounts1_ID = to.ID,
                DebitedAmount      = dto.Amount,
                CreditedAmount     = 0,
                Comments           = dto.Comments
            };
            var creditLine = new TransactionLine {
                Transaction_ID     = txn.ID,
                MasterAccounts_ID  = to.ID,
                MasterAccounts1_ID = from.ID,
                DebitedAmount      = 0,
                CreditedAmount     = dto.Amount,
                Comments           = dto.Comments
            };
            _context.TransactionLines.AddRange(debitLine, creditLine);

            // 3) Update the closing balances on both accounts
            from.ClosingAmount -= dto.Amount;
            to.ClosingAmount   += dto.Amount;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Populates the ViewBag.Accounts dropdown with the current user's master accounts.
        /// </summary>
        private async Task PopulateAccountsDropdown()
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            
            // Load accounts including their group relationship
            var accts = await _context.MasterAccounts
                .Include(ma => ma.Group)
                .Where(ma => ma.Group.CreatedByUserID == userId)
                .ToListAsync();

            ViewBag.Accounts = new SelectList(accts, "ID", "Name");
        }
    }
}
