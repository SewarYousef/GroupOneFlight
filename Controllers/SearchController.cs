using GroupOneFlight.Areas.Airlines.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupOneFlight.Controllers
{
    public class SearchController : Controller
    {
        private readonly AirBnBContext _context;
        private readonly IHttpContextAccessor _accessor;

        public SearchController(AirBnBContext context, IHttpContextAccessor accessor)
        {
            _context  = context;
            _accessor = accessor;
        }

        private FlightSession FlightSession => new(HttpContext.Session);
        private FlightCookie  FlightCookie  => new(_accessor);

        // GET /Search/Index
        public IActionResult Index()
        {
            if (!FlightSession.GetSelectedFlights().Any())
            {
                var cookieIds = FlightCookie.GetSelectedFlights();
                if (cookieIds.Any()) FlightSession.SetSelectedFlights(cookieIds);
            }

            return View(BuildViewModel(FlightSession.GetFilter()));
        }

        // POST /Search/Index — save filter, PRG redirect
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(FlightFilter filter)
        {
            FlightSession.SetFilter(filter);
            return RedirectToAction(nameof(Index));
        }

        // GET /Search/ClearFilter — clears all filter values from session, redirects to Index
        public IActionResult ClearFilter()
        {
            FlightSession.SetFilter(new FlightFilter());
            return RedirectToAction(nameof(Index));
        }

        // GET /Search/Details/5
        public IActionResult Details(int id)
        {
            var flight = _context.Flights.Include(f => f.Airline).FirstOrDefault(f => f.Id == id);
            if (flight == null) return NotFound();

            ViewBag.IsSelected     = FlightSession.GetSelectedFlights().Contains(id);
            ViewBag.SelectionCount = FlightSession.SelectionCount;
            return View(flight);
        }

        // POST /Search/Select — PRG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Select(int flightId)
        {
            var flight = _context.Flights.FirstOrDefault(f => f.Id == flightId);
            if (flight == null) return NotFound();

            FlightSession.AddFlight(flightId);
            FlightCookie.AddFlight(flightId);

            TempData["Confirmation"] =
                $"Flight {flight.FlightNumber} selected! You have {FlightSession.SelectionCount} flight(s).";

            return RedirectToAction(nameof(Index));
        }

        // GET /Search/Selections
        public IActionResult Selections()
        {
            var ids = FlightSession.GetSelectedFlights();
            var flights = _context.Flights.Include(f => f.Airline)
                                          .Where(f => ids.Contains(f.Id)).ToList();
            ViewBag.SelectionCount = FlightSession.SelectionCount;
            return View(flights);
        }

        // POST /Search/RemoveFlight — PRG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFlight(int flightId)
        {
            var flight = _context.Flights.FirstOrDefault(f => f.Id == flightId);
            FlightSession.RemoveFlight(flightId);
            FlightCookie.RemoveFlight(flightId);
            TempData["Confirmation"] = flight != null
                ? $"Flight {flight.FlightNumber} removed from selections."
                : "Flight removed.";
            return RedirectToAction(nameof(Selections));
        }

        // POST /Search/ClearAll — PRG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearAll()
        {
            FlightSession.ClearSelections();
            FlightCookie.ClearSelections();
            TempData["Confirmation"] = "All selections cleared.";
            return RedirectToAction(nameof(Selections));
        }

        private SearchViewModel BuildViewModel(FlightFilter filter)
        {
            var query = _context.Flights.Include(f => f.Airline).AsQueryable();

            if (!string.IsNullOrEmpty(filter.From))
                query = query.Where(f => f.From == filter.From);
            if (!string.IsNullOrEmpty(filter.To))
                query = query.Where(f => f.To == filter.To);
            if (filter.DepartureDate.HasValue)
                query = query.Where(f => f.Date.Date == filter.DepartureDate.Value.Date);
            if (!string.IsNullOrEmpty(filter.CabinType) && filter.CabinType != "All")
                query = query.Where(f => f.CabinType == filter.CabinType);

            return new SearchViewModel
            {
                Flights        = query.ToList(),
                Airlines       = _context.Airlines.ToList(),
                Filter         = filter,
                SelectionCount = FlightSession.SelectionCount,
                FromCities     = _context.Flights.Select(f => f.From).Where(c => c != null).Distinct().OrderBy(c => c).Select(c => c!).ToList(),
                ToCities       = _context.Flights.Select(f => f.To).Where(c => c != null).Distinct().OrderBy(c => c).Select(c => c!).ToList(),
            };
        }
    }
}
