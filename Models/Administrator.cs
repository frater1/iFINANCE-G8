using System;
using System.ComponentModel.DataAnnotations;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// Represents an administrator user in the iFinance system, storing hire and termination dates
    /// and linking to the associated user credentials.
    /// </summary>
    public class Administrator
    {
        /// <summary>
        /// Primary key identifier for the administrator record.
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Full name of the administrator.
        /// </summary>
        [Required, StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// The date when the administrator was hired. Optional if not yet hired.
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? DateHired { get; set; }

        /// <summary>
        /// The date when the administrator's tenure ended. Optional if still active.
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? DateFinished { get; set; }

        /// <summary>
        /// Foreign key referencing the associated user credentials (1:1 relationship).
        /// </summary>
        public int UserPassword_ID { get; set; }

        /// <summary>
        /// Navigation property to the associated UserPassword entity for authentication.
        /// </summary>
        public UserPassword UserPassword { get; set; }
    }
}
