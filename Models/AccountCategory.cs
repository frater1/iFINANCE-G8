using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// Represents a category for grouping accounts, such as Assets, Liabilities, Income, or Expenses.
    /// </summary>
    public class AccountCategory
    {
        /// <summary>
        /// Primary key identifier for the AccountCategory.
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// The display name of the category (e.g., "Assets", "Liabilities").
        /// </summary>
        [Required, StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// The type classification of the category, used for report logic (e.g., "Asset", "Liability").
        /// Initialized to an empty string to prevent null values.
        /// </summary>
        [Required, StringLength(50)]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property for the groups associated with this category.
        /// </summary>
        public ICollection<Group> Groups { get; set; }
    }
}
