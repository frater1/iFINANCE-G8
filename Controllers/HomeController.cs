using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Group8_iFINANCE_APP.Models;

namespace Group8_iFINANCE_APP.Controllers;

/// <summary>
/// Handles requests to general, non-secured pages such as the home and privacy views,
/// as well as error diagnostics.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController"/> class with a logger.
    /// </summary>
    /// <param name="logger">The logger instance for logging information and errors.</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Serves the application home page.
    /// </summary>
    /// <returns>The Index view.</returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Serves the privacy policy page.
    /// </summary>
    /// <returns>The Privacy view.</returns>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Handles and displays error information for unhandled exceptions.
    /// Caches no response to ensure fresh diagnostic data.
    /// </summary>
    /// <returns>The Error view with an <see cref="ErrorViewModel"/> containing the request ID.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Populate the view model with the current activity or HTTP trace identifier
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return View(new ErrorViewModel { RequestId = requestId });
    }
}
