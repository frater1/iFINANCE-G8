using System.Collections.Generic;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// Wraps one Group plus its child groups and all the MasterAccounts in it.
    /// </summary>
    public class GroupWithAccounts
    {
        public Group Group { get; set; } = null!;

        public List<MasterAccount> Accounts { get; set; } = new();

        public List<GroupWithAccounts> Children { get; set; } = new();
    }
}