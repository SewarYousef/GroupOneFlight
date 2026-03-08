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
    }
}