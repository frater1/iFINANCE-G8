using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// Represents a single line item within a transaction, detailing debit and credit entries.
    /// </summary>
    public class TransactionLine
    {
        /// <summary>
        /// Primary key identifier for the transaction line.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Foreign key to the master account being debited.
        /// </summary>
        public int MasterAccounts_ID { get; set; }

        /// <summary>
        /// Navigation property for the debited master account.
        /// </summary>
        public MasterAccount MasterAccount { get; set; }

        /// <summary>
        /// Foreign key to the master account being credited.
        /// </summary>
        public int MasterAccounts1_ID { get; set; }

        /// <summary>
        /// Navigation property for the credited master account.
        /// </summary>
        public MasterAccount MasterAccount1 { get; set; }

        /// <summary>
        /// Foreign key linking back to the parent transaction.
        /// </summary>
        public int Transaction_ID { get; set; }

        /// <summary>
        /// Navigation property for the parent transaction.
        /// </summary>
        public Transaction Transaction { get; set; }

        /// <summary>
        /// The amount debited to the <see cref="MasterAccount"/> in this line.
        /// </summary>
        public double DebitedAmount { get; set; }

        /// <summary>
        /// The amount credited from the <see cref="MasterAccount1"/> in this line.
        /// </summary>
        public double CreditedAmount { get; set; }

        /// <summary>
        /// Optional comments or memo for this transaction line.
        /// </summary>
        public string? Comments { get; set; }
    }
}
