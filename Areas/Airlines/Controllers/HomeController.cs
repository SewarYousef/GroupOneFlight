using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GroupOneFlight.Areas.Airlines.ViewModels;
using GroupOneFlight.Areas.Airlines.Models;
using System.Linq;

namespace GroupOneFlight.Areas.Airlines.Controllers
{
    [Area("Airlines")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AirBnBContext _context;

        public HomeController(ILogger<HomeController> logger, AirBnBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string? fromCity, string? toCity, string? cabinType)
        {
            // Load previous session values if current filters are empty
            if (string.IsNullOrEmpty(fromCity) && HttpContext.Session.GetString("SelectedFrom") != null)
                fromCity = HttpContext.Session.GetString("SelectedFrom")!;

            if (string.IsNullOrEmpty(toCity) && HttpContext.Session.GetString("SelectedTo") != null)
                toCity = HttpContext.Session.GetString("SelectedTo")!;

            if (string.IsNullOrEmpty(cabinType) && HttpContext.Session.GetString("SelectedCabin") != null)
                cabinType = HttpContext.Session.GetString("SelectedCabin")!;

            // Query flights with filters - include both Airline and FlightOptions
            var flightsQuery = _context.Flights
                .Include(f => f.Airline)
                .AsQueryable();

            if (!string.IsNullOrEmpty(fromCity))
                flightsQuery = flightsQuery.Where(f => f.From == fromCity);

            if (!string.IsNullOrEmpty(toCity))
                flightsQuery = flightsQuery.Where(f => f.To == toCity);

            if (!string.IsNullOrEmpty(cabinType))
                flightsQuery = flightsQuery.Where(f => f.CabinType == cabinType);

            var flights = flightsQuery.ToList();

            // Load FlightOptions for pricing information
            var flightOptions = _context.FlightOptions.ToList();
            var minPriceByFlightId = new Dictionary<int, decimal>();

            foreach (var option in flightOptions)
            {
                decimal minPrice = Math.Min(
                    Math.Min(option.EconomyPrice, option.BusinessPrice),
                    option.FirstClassPrice
                );
                minPriceByFlightId[option.FlightId] = minPrice;
            }

            // Save filters in session
            HttpContext.Session.SetString("SelectedFrom", fromCity ?? string.Empty);
            HttpContext.Session.SetString("SelectedTo", toCity ?? string.Empty);
            HttpContext.Session.SetString("SelectedCabin", cabinType ?? string.Empty);

            // Build ViewModel with null-safe lists
            var viewModel = new FlightViewModel
            {
                Flights = flights,
                Airlines = _context.Airlines.ToList(),
                FlightOptions = flightOptions,
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
                     .ToList(),
                AircraftTypes = AircraftTypes.GetAllAircraftTypes(),
                SelectedFrom = fromCity,
                SelectedTo = toCity,
                SelectedCabin = cabinType,
                MinPriceByFlightId = minPriceByFlightId
            };

            ViewData["SelectedFrom"] = fromCity;
            ViewData["SelectedTo"] = toCity;
            ViewData["SelectedCabin"] = cabinType;

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}