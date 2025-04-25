using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// Represents a user-defined account grouping within the iFinance application,
    /// supporting hierarchical organization (parent/child) and navigation properties.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Primary key identifier for the group.
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// The display name of the group.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key linking to the account category this group belongs to.
        /// </summary>
        [Display(Name = "Category")] 
        [Required]
        public int AccountCategory_ID { get; set; }

        /// <summary>
        /// Foreign key for the parent group, allowing nested group hierarchies.
        /// Nullable for top-level (root) groups.
        /// </summary>
        [Display(Name = "Parent Group")]
        public int? parent_ID { get; set; }

        /// <summary>
        /// The ID of the user who created this group, linking to the NonAdminUser.
        /// </summary>
        [Display(Name = "Created by")]
        public int CreatedByUserID { get; set; }

        // ───── Skip MVC binding/validation on all navigation properties ─────

        /// <summary>
        /// Navigation property for the related account category.
        /// </summary>
        [ValidateNever]
        public AccountCategory? AccountCategory { get; set; }

        /// <summary>
        /// Navigation property for the parent group entity.
        /// </summary>
        [ValidateNever]
        public Group? ParentGroup { get; set; }

        /// <summary>
        /// Navigation property for child groups nested under this group.
        /// </summary>
        [ValidateNever]
        public ICollection<Group>? Children { get; set; }

        /// <summary>
        /// Navigation property for master accounts assigned to this group.
        /// </summary>
        [ValidateNever]
        public ICollection<MasterAccount>? MasterAccounts { get; set; }
    }
}
