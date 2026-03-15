using Microsoft.AspNetCore.Mvc;

namespace GroupOneFlight.Areas.Airline.Controllers
{
    [Area("Airline")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}