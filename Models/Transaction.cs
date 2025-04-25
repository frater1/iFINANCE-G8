using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// Represents a financial transaction header, which groups multiple transaction lines
    /// and ties the transaction to a non-admin user.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Primary key identifier for the transaction.
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// The date the transaction took place. Required for all transactions.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        /// <summary>
        /// Optional description or memo for the transaction (up to 200 characters).
        /// </summary>
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string? Description { get; set; }

        /// <summary>
        /// Foreign key referencing the non-admin user who created or owns this transaction.
        /// </summary>
        public int NonAdminUser_ID { get; set; }

        /// <summary>
        /// Navigation property for the user who owns this transaction.
        /// </summary>
        public NonAdminUser NonAdminUser { get; set; } = null!;

        /// <summary>
        /// Collection of line items that make up the debits and credits of this transaction.
        /// </summary>
        public ICollection<TransactionLine> TransactionLines { get; set; }
            = new List<TransactionLine>();
    }
}
