using Microsoft.AspNetCore.Mvc;

namespace YourProjectName.Areas.Airline.Controllers
{
    [Area("Airline")]
    public class AirlineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Manage()
        {
            return Content("Manage Flights - Testing Route");
        }

        public IActionResult Regulation()
        {
            return Content("Regulation - Testing Route");
        }
    }
}