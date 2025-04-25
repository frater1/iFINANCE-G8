namespace Group8_iFINANCE_APP.Models
{
    /// <summary>
    /// ViewModel for error views, capturing the current request ID
    /// and whether to display it for diagnostic purposes.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// The unique identifier for the current HTTP request.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Indicates whether the RequestId should be shown in the view.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
