using Microsoft.AspNetCore.Mvc;

namespace GroupOneFlight.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
         public IActionResult Index()
        {
            return View();
        }

        public ContentResult Manage()
        {
            return Content("Admin Area – Manage Users");
        }

        public ContentResult Rights()
        {
            return Content("Admin Area – Rights & Obligations");
        }
    }
}
