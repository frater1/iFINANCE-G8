using System;

namespace Group8_iFINANCE_APP.Models
{
    public class TransactionIndexViewModel
    {
        public int      ID          { get; set; }
        public DateTime Date        { get; set; }
        public string?  Description { get; set; }
        public double   Amount      { get; set; }
    }
}