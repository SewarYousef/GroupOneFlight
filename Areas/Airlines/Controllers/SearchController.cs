using Microsoft.AspNetCore.Mvc;

namespace GroupOneFlight.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Flight Search";
            return View();  
        }
    }
}