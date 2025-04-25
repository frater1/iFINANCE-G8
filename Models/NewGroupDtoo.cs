namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// Data Transfer Object for creating a new group with a specified name and category.
    /// </summary>
    public class NewGroupDtoo
    {
        /// <summary>
        /// The display name for the new group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The category identifier under which the group will be created.
        /// </summary>
        public int CategoryID { get; set; }
    }
}
