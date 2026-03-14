using Microsoft.AspNetCore.Mvc;

namespace GroupOneFlight.Areas.Airline.Controllers
{
    [Area("Airline")]
    public class AirlineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ContentResult Manage()
        {
            return Content("Airline Area - Manage Flights");
        }

        public ContentResult Regulation()
        {
            return Content("Airline Area - Regulation");
        }
    }
}
