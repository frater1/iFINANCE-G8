using System;
using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// Data Transfer Object for creating a new financial transaction,
    /// capturing header details and required account identifiers and amounts.
    /// </summary>
    public class TransactionCreateDto
    {
        /// <summary>
        /// The date the transaction occurs. Defaults to today's date.
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
        /// Identifier of the source (debit) account for the transaction.
        /// </summary>
        [Required]
        [Display(Name = "From Account")]
        public int DebitAccountId { get; set; }

        /// <summary>
        /// Identifier of the destination (credit) account for the transaction.
        /// </summary>
        [Required]
        [Display(Name = "To Account")]
        public int CreditAccountId { get; set; }

        /// <summary>
        /// The amount to transfer from the debit to the credit account.
        /// Must be greater than zero.
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public double Amount { get; set; }

        /// <summary>
        /// Optional comments for each transaction line, up to 200 characters.
        /// </summary>
        [StringLength(200, ErrorMessage = "Comments cannot exceed 200 characters.")]
        public string? Comments { get; set; }
    }
}
