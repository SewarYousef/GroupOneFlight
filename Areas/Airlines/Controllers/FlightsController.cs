using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupOneFlight.Areas.Airlines.Models;
using GroupOneFlight.Areas.Airlines.ViewModels;

namespace GroupOneFlight.Areas.Airlines.Controllers
{
    [Area("Airlines")]
    public class FlightsController : Controller
    {
        private readonly AirBnBContext _context;

        public FlightsController(AirBnBContext context)
        {
            _context = context;
        }

        // Airline Dashboard (Flights page)
        public IActionResult Index()
        {
            var viewModel = new FlightViewModel
            {
                Flights = _context.Flights
                                  .Include(f => f.Airline)
                                  .ToList(),

                FromCities = _context.Flights
                                     .Select(f => f.From)
                                     .Where(f => f != null)
                                     .Distinct()
                                     .Select(f => f!)
                                     .ToList(),

                ToCities = _context.Flights
                                   .Select(f => f.To)
                                   .Where(f => f != null)
                                   .Distinct()
                                   .Select(f => f!)
                                   .ToList(),

                CabinTypes = _context.Flights
                                     .Select(f => f.CabinType)
                                     .Where(c => c != null)
                                     .Distinct()
                                     .Select(c => c!)
                                     .ToList()
            };

            return View(viewModel);
        }

        // Manage Flights (Routing test)
        public IActionResult Manage()
        {
            return Content("Airline Manage Flights Page");
        }

        // Regulation Information (Routing test)
        public IActionResult Regulation()
        {
            return Content("Airline Regulation Page");
        }
    }
}