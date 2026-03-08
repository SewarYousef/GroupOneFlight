using Microsoft.AspNetCore.Mvc;

namespace GroupOneFlight.Areas.Client.Controllers
{
    [Area("Client")]
    public class ClientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}