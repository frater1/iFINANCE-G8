using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// ViewModel representing a full transaction with header information and multiple line items,
    /// used for both display and creation/editing in the UI.
    /// </summary>
    public class TransactionViewModel
    {
        /// <summary>
        /// Optional identifier for the transaction; null when creating a new transaction.
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// The date of the transaction. Defaults to today's date when creating a new transaction.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        /// <summary>
        /// Optional description or memo for the transaction, up to 200 characters.
        /// </summary>
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string? Description { get; set; }

        /// <summary>
        /// Collection of line items (debits and credits) associated with this transaction.
        /// </summary>
        public List<TransactionLineViewModel> Lines { get; set; }
            = new List<TransactionLineViewModel>();

        /// <summary>
        /// Ensures at least one line item exists in the transaction, adding an empty line if needed.
        /// Useful to initialize the form with one editable line.
        /// </summary>
        public void EnsureAtLeastOneLine()
        {
            if (Lines.Count == 0)
                Lines.Add(new TransactionLineViewModel());
        }
    }
}
