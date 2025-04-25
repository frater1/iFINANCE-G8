using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// ViewModel for creating a new master account, including options to select or define a group.
    /// </summary>
    public class MasterAccountCreateViewModel
    {
        /// <summary>
        /// The display name for the master account.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The opening balance for the account. Must be zero or positive.
        /// </summary>
        [Display(Name = "Opening balance")]
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Opening balance must be non-negative.")]
        public double OpeningAmount { get; set; }

        /// <summary>
        /// The selected category ID for grouping this account.
        /// </summary>
        [Display(Name = "Category")]
        [Required]
        public int CategoryID { get; set; }

        /// <summary>
        /// The ID of an existing group to assign this account to (optional).
        /// </summary>
        [Display(Name = "Account Group")]
        public int? GroupID { get; set; }

        /// <summary>
        /// Name of a new group to create if the user does not select an existing one (optional).
        /// </summary>
        [Display(Name = "New Group name")]
        public string? NewGroupName { get; set; }

        /// <summary>
        /// The ID of a parent group for nesting a newly created group (optional).
        /// </summary>
        public int? ParentGroupID { get; set; }

        /// <summary>
        /// Select list for account categories, populated by the controller.
        /// </summary>
        public SelectList? Categories { get; set; }

        /// <summary>
        /// Select list for existing groups, populated by the controller.
        /// </summary>
        public SelectList? ExistingGroups { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for creating a new group outside of master account context.
    /// </summary>
    public class NewGroupDto
    {
        /// <summary>
        /// The name of the new group.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The category ID under which to create the new group.
        /// </summary>
        [Required]
        public int CategoryID { get; set; }
    }
}
