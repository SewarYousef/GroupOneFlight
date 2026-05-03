using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupOneFlight.Models.DataLayer;
using GroupOneFlight.Models.ViewModels;
using GroupOneFlight.Models.DomainModels;

namespace GroupOneFlight.Controllers
{
    public class HomeController : Controller
    {
        private readonly FlightDbContext _context;

        public HomeController(FlightDbContext context)
        {
            _context = context;
        }

        // THIS is the important part
        public IActionResult Index(string? fromCity, string? toCity, string? cabinType)
        {
            var flightsQuery = _context.Flights
                .Include(f => f.Airline)
                .AsQueryable();

            // Filters
            if (!string.IsNullOrEmpty(fromCity))
                flightsQuery = flightsQuery.Where(f => f.From == fromCity);

            if (!string.IsNullOrEmpty(toCity))
                flightsQuery = flightsQuery.Where(f => f.To == toCity);

            if (!string.IsNullOrEmpty(cabinType))
                flightsQuery = flightsQuery.Where(f => f.CabinType == cabinType);

            var flights = flightsQuery.OrderBy(f => f.Date).ToList();

            // FlightOptions
            var flightOptions = _context.FlightOptions.ToList();

            // Min price per flight
            var minPrices = flightOptions
                .GroupBy(o => o.FlightId)
                .ToDictionary(
                    g => g.Key,
                    g => new[]
                    {
                        g.Min(x => x.EconomyPrice),
                        g.Min(x => x.BusinessPrice),
                        g.Min(x => x.FirstClassPrice)
                    }.Min()
                );

            var viewModel = new FlightIndexViewModel
            {
                Flights = flights,
                FlightOptions = flightOptions,
                MinPriceByFlightId = minPrices,

                FromCities = _context.Flights
                    .Select(f => f.From)
                    .Where(f => f != null)
                    .Distinct()
                    .OrderBy(c => c)
                    .Select(f => f!)
                    .ToList(),

                ToCities = _context.Flights
                    .Select(f => f.To)
                    .Where(f => f != null)
                    .Distinct()
                    .OrderBy(c => c)
                    .Select(f => f!)
                    .ToList(),

                CabinTypes = CabinTypes.GetAll(),

                // Keep selected values
                SelectedFrom = fromCity,
                SelectedTo = toCity,
                SelectedCabin = cabinType
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}