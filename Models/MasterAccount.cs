using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// Represents an individual ledger account within a grouped category,
    /// storing balances and linking to its owner and group.
    /// </summary>
    public class MasterAccount
    {
        /// <summary>
        /// Primary key identifier for the master account.
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Display name of the account (e.g., "Cash", "Accounts Receivable").
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Account name cannot exceed 100 characters.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The opening balance when the account was created.
        /// </summary>
        public double? OpeningAmount { get; set; }

        /// <summary>
        /// The current or closing balance of the account.
        /// </summary>
        public double? ClosingAmount { get; set; }

        /// <summary>
        /// Foreign key linking this account to its parent group.
        /// </summary>
        public int Group_ID { get; set; }

        /// <summary>
        /// Navigation property for the group this account belongs to.
        /// </summary>
        public Group Group { get; set; } = null!;

        /// <summary>
        /// Foreign key indicating the non-admin user who owns this account.
        /// </summary>
        public int NonAdminUser_ID { get; set; }

        /// <summary>
        /// Navigation property for the user who owns this account.
        /// </summary>
        public NonAdminUser NonAdminUser { get; set; } = null!;

        /// <summary>
        /// Collection of transaction lines associated with this master account.
        /// </summary>
        public ICollection<TransactionLine> TransactionLines { get; set; }
            = new List<TransactionLine>();
    }
}
