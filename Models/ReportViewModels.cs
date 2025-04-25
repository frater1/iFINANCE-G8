using System;
using System.Collections.Generic;
using System.Linq;

namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// Represents a single line item in a financial report,
    /// containing the account name and its debit and credit values.
    /// </summary>
    public class ReportLineItem
    {
        /// <summary>
        /// The display name of the account for this line item.
        /// </summary>
        public string AccountName { get; set; } = string.Empty;

        /// <summary>
        /// The total debit amount for the account within the report context.
        /// </summary>
        public double Debit { get; set; }

        /// <summary>
        /// The total credit amount for the account within the report context.
        /// </summary>
        public double Credit { get; set; }
    }

    /// <summary>
    /// ViewModel for passing report data to views, including date range and line items.
    /// </summary>
    public class ReportViewModel
    {
        /// <summary>
        /// The starting date of the report's time range.
        /// </summary>
        public DateTime From { get; set; }

        /// <summary>
        /// The ending date of the report's time range.
        /// </summary>
        public DateTime To { get; set; }

        /// <summary>
        /// The collection of report line items to display.
        /// </summary>
        public List<ReportLineItem> Lines { get; set; } = new List<ReportLineItem>();

        /// <summary>
        /// Sum of all debit values in the report lines.
        /// </summary>
        public double TotalDebit => Lines.Sum(l => l.Debit);

        /// <summary>
        /// Sum of all credit values in the report lines.
        /// </summary>
        public double TotalCredit => Lines.Sum(l => l.Credit);
    }
}
