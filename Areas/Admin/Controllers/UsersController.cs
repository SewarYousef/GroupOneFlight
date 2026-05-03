using Microsoft.AspNetCore.Mvc;


namespace GroupOneFlight.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        // Admin Dashboard
        public IActionResult Index()
        {
            return View();
        }

        // Manage Users (Routing Test)
        public IActionResult Manage()
        {
            return Content("Admin Area – Manage Users");
        }

        // Rights & Obligations (Routing Test)
        public IActionResult Rights()
        {
            return Content("Admin Area – Rights & Obligations");
        }
    }
}