using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// ViewModel for capturing individual transaction line inputs within forms,
    /// enforcing validation on account selection and amounts.
    /// </summary>
    public class TransactionLineViewModel
    {
        /// <summary>
        /// Identifier of the master account to which this line applies.
        /// </summary>
        [Required(ErrorMessage = "Account selection is required.")]
        [Display(Name = "Account")]
        public int MasterAccountID { get; set; }

        /// <summary>
        /// Amount to debit from the specified account. Must be zero or positive.
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Debit amount cannot be negative.")]
        [Display(Name = "Debited Amount")]
        public double DebitedAmount { get; set; }

        /// <summary>
        /// Amount to credit to the specified account. Must be zero or positive.
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Credit amount cannot be negative.")]
        [Display(Name = "Credited Amount")]
        public double CreditedAmount { get; set; }

        /// <summary>
        /// Optional comments for the transaction line (max. 200 characters).
        /// </summary>
        [StringLength(200, ErrorMessage = "Comments cannot exceed 200 characters.")]
        [Display(Name = "Comments")]
        public string? Comments { get; set; }
    }
}
