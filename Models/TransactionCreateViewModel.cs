using System;
using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// ViewModel for creating a new transaction via the UI, encapsulating form inputs
    /// and enforcing validation rules for transaction creation.
    /// </summary>
    public class TransactionCreateViewModel
    {
        /// <summary>
        /// The date when the transaction is recorded. Defaults to today's date.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        /// <summary>
        /// Optional description or memo for the transaction (max. 200 characters).
        /// </summary>
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string? Description { get; set; }

        /// <summary>
        /// Identifier of the account to debit (source account).
        /// </summary>
        [Required]
        [Display(Name = "From Account")]
        public int DebitAccountId { get; set; }

        /// <summary>
        /// Identifier of the account to credit (destination account).
        /// </summary>
        [Required]
        [Display(Name = "To Account")]
        public int CreditAccountId { get; set; }

        /// <summary>
        /// The amount to transfer from the debit account to the credit account.
        /// Must be greater than zero.
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public double Amount { get; set; }

        /// <summary>
        /// Optional comments for each transaction line (max. 200 characters).
        /// </summary>
        [StringLength(200, ErrorMessage = "Comments cannot exceed 200 characters.")]
        public string? Comments { get; set; }
    }
}
