using Microsoft.AspNetCore.Mvc;

namespace GroupOneFlight.Areas.Client.Controllers
{
    [Area("Client")]
    public class ClientController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Dashboard page
        }

        public IActionResult Search()
        {
            return Content("Search Page");
        }

        public IActionResult Privacy()
        {
            return Content("Privacy Page");
        }
    }
}