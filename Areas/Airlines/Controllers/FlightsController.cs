using Microsoft.AspNetCore.Mvc;

namespace GroupOneFlight.Areas.Airlines.Controllers
{
    [Area("Airlines")]
    public class FlightsController : Controller
    {
        // Airline Dashboard
        public IActionResult Index()
        {
            return View();
        }

        // Manage Flights (Routing Test)
        public IActionResult Manage()
        {
            return Content("Airline Manage Flights Page");
        }

        // Regulation Information (Routing Test)
        public IActionResult Regulation()
        {
            return Content("Airline Regulation Page");
        }
    }
}