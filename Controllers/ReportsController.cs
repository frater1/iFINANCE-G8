using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Group8_iFINANCE_APP.Data;
using Group8_iFINANCE_APP.Models;

namespace Group8_iFINANCE_APP.Controllers
{
    /// <summary>
    /// Provides financial report endpoints including Trial Balance, Balance Sheet, Profit & Loss, and Cash Flow stubs.
    /// </summary>
    public class ReportsController : Controller
    {
        private readonly Group8_iFINANCEAPP_DBContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportsController"/> class with the given database context.
        /// </summary>
        /// <param name="db">The EF Core context for accessing financial data.</param>
        public ReportsController(Group8_iFINANCEAPP_DBContext db) => _db = db;

        /// <summary>
        /// Displays the reports landing page.
        /// </summary>
        /// <returns>The default Index view.</returns>
        // GET: /Reports
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Ensures the date range parameters have valid values and correct order.
        /// Defaults to the past month if dates are null, and swaps if from > to.
        /// </summary>
        /// <param name="from">The start date of the range (nullable).</param>
        /// <param name="to">The end date of the range (nullable).</param>
        private void EnsureDates(ref DateTime? from, ref DateTime? to)
        {
            // Default 'from' to one month ago if not provided
            from ??= DateTime.Today.AddMonths(-1);
            // Default 'to' to today if not provided
            to   ??= DateTime.Today;
            // Swap if the provided range is inverted
            if (from > to) (from, to) = (to, from);
        }

        // ─── Trial Balance ───────────────────────────────────

        /// <summary>
        /// Generates a trial balance report for the given date range.
        /// </summary>
        /// <param name="from">Start date (nullable).</param>
        /// <param name="to">End date (nullable).</param>
        /// <returns>The TrialBalance view with calculated debit/credit lines.</returns>
        [HttpGet]
        public async Task<IActionResult> TrialBalance(DateTime? from, DateTime? to)
        {
            // Retrieve current user's ID from session
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            // Ensure valid date range parameters
            EnsureDates(ref from, ref to);

            // Load all master accounts owned by this user
            var accounts = await _db.MasterAccounts
                .Include(ma => ma.Group)
                .Where(ma => ma.NonAdminUser_ID == userId)
                .ToListAsync();

            // Initialize view model with date range
            var vm = new ReportViewModel { From = from.Value, To = to.Value };

            // For each account, sum debit and credit transactions within the range
            foreach (var acct in accounts)
            {
                var lines = await _db.TransactionLines
                   .Where(tl => tl.Transaction.NonAdminUser_ID == userId
                             && tl.MasterAccounts_ID == acct.ID
                             && tl.Transaction.Date >= vm.From
                             && tl.Transaction.Date <= vm.To)
                   .ToListAsync();

                // Calculate totals for the account
                double debitSum  = lines.Sum(l => l.DebitedAmount);
                double creditSum = lines.Sum(l => l.CreditedAmount);

                // Add a line item to the view model
                vm.Lines.Add(new ReportLineItem {
                    AccountName = acct.Name,
                    Debit  = debitSum,
                    Credit = creditSum
                });
            }

            return View(vm);
        }

        // ─── Balance Sheet ──────────────────────────────────

        /// <summary>
        /// Generates a balance sheet report as of the specified date.
        /// </summary>
        /// <param name="asOf">The date for which to report balances (nullable).</param>
        /// <returns>The BalanceSheet view with asset and liability lines.</returns>
        [HttpGet]
        public async Task<IActionResult> BalanceSheet(DateTime? asOf)
        {
            // Retrieve current user's ID from session
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            // Default to today if no date provided
            var date   = asOf ?? DateTime.Today;

            // Initialize view model with identical From/To for 'as of' logic
            var vm = new ReportViewModel { From = date, To = date };

            // Load accounts and include category information
            var accounts = await _db.MasterAccounts
                .Include(ma => ma.Group).ThenInclude(g => g.AccountCategory)
                .Where(ma => ma.NonAdminUser_ID == userId)
                .ToListAsync();

            // Filter for Assets and Liabilities and report their balances
            foreach (var acct in accounts)
            {
                double balance = acct.ClosingAmount ?? 0;
                var cat = acct.Group.AccountCategory.Name;

                if (cat == "Assets" || cat == "Liabilities")
                {
                    vm.Lines.Add(new ReportLineItem {
                        AccountName = $"{acct.Name} ({cat})",
                        Debit  = cat == "Assets" ? balance : 0,
                        Credit = cat == "Liabilities" ? balance : 0
                    });
                }
            }

            return View(vm);
        }

        // ─── Profit & Loss ──────────────────────────────────

        /// <summary>
        /// Generates a profit and loss report for the given date range.
        /// </summary>
        /// <param name="from">Start date (nullable).</param>
        /// <param name="to">End date (nullable).</param>
        /// <returns>The ProfitLoss view with income and expense lines.</returns>
        [HttpGet]
        public async Task<IActionResult> ProfitLoss(DateTime? from, DateTime? to)
        {
            // Retrieve current user's ID from session
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            // Ensure valid date range parameters
            EnsureDates(ref from, ref to);

            // Initialize view model
            var vm = new ReportViewModel { From = from.Value, To = to.Value };

            // Include category for income/expense classification
            var accounts = await _db.MasterAccounts
                .Include(ma => ma.Group).ThenInclude(g => g.AccountCategory)
                .Where(ma => ma.NonAdminUser_ID == userId)
                .ToListAsync();

            // Process only Income and Expenses accounts
            foreach (var acct in accounts)
            {
                var cat = acct.Group.AccountCategory.Name;
                if (cat == "Income" || cat == "Expenses")
                {
                    var lines = await _db.TransactionLines
                       .Where(tl => tl.Transaction.NonAdminUser_ID == userId
                                 && tl.MasterAccounts_ID == acct.ID
                                 && tl.Transaction.Date >= vm.From
                                 && tl.Transaction.Date <= vm.To)
                       .ToListAsync();

                    double debitSum  = lines.Sum(l => l.DebitedAmount);
                    double creditSum = lines.Sum(l => l.CreditedAmount);

                    vm.Lines.Add(new ReportLineItem {
                        AccountName = acct.Name,
                        Debit  = cat == "Expenses" ? debitSum : 0,
                        Credit = cat == "Income"   ? creditSum : 0
                    });
                }
            }

            return View(vm);
        }

        // ─── Cash Flow ──────────────────────────────────────

        /// <summary>
        /// Stub for cash flow report; to be implemented in a more detailed manner.
        /// </summary>
        /// <returns>A placeholder NotYet view.</returns>
        [HttpGet]
        public IActionResult CashFlow()
        {
            // Future implementation: categorize and summarize cash movements
            return View("NotYet");
        }
    }
}
