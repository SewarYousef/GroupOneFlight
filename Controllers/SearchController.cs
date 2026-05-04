using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using GroupOneFlight.Models.ViewModels;
using GroupOneFlight.Models.DomainModels;
using GroupOneFlight.Models.DataLayer.Repositories;

namespace GroupOneFlight.Controllers
{
    public class SearchController : Controller
    {
        private readonly IFlightRepository        _flights;
        private readonly IRepository<Reservation> _reservations;
        private readonly IRepository<Airline>     _airlines;

        public SearchController(
            IFlightRepository        flights,
            IRepository<Reservation> reservations,
            IRepository<Airline>     airlines)
        {
            _flights      = flights;
            _reservations = reservations;
            _airlines     = airlines;
        }

        // =========================
        // SESSION / COOKIE HELPERS
        // =========================

        private const string SESSION_KEY = "SelectedFlights";
        private const string COOKIE_KEY  = "SelectedFlightsCookie";

        private List<int> GetSessionFlights()
        {
            var data = HttpContext.Session.GetString(SESSION_KEY);
            return string.IsNullOrEmpty(data)
                ? new List<int>()
                : JsonSerializer.Deserialize<List<int>>(data) ?? new List<int>();
        }

        private void SaveSessionFlights(List<int> flights) =>
            HttpContext.Session.SetString(SESSION_KEY, JsonSerializer.Serialize(flights));

        private List<int> GetCookieFlights()
        {
            var cookie = Request.Cookies[COOKIE_KEY];
            return string.IsNullOrEmpty(cookie)
                ? new List<int>()
                : JsonSerializer.Deserialize<List<int>>(cookie) ?? new List<int>();
        }

        private void SaveCookieFlights(List<int> flights) =>
            Response.Cookies.Append(COOKIE_KEY, JsonSerializer.Serialize(flights),
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(7), HttpOnly = true });

        // =========================
        // INDEX
        // =========================

        public IActionResult Index()
        {
            var sessionFlights = GetSessionFlights();
            if (!sessionFlights.Any())
            {
                var cookieFlights = GetCookieFlights();
                if (cookieFlights.Any()) SaveSessionFlights(cookieFlights);
            }
            return View(BuildViewModel(new FlightFilter()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(FlightFilter filter)
        {
            HttpContext.Session.SetString("Filter", JsonSerializer.Serialize(filter));
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ClearFilter()
        {
            HttpContext.Session.Remove("Filter");
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS
        // =========================

        public IActionResult Details(int id)
        {
            var flight = _flights.Get(new QueryOptions<Flight>
            {
                Where    = f => f.Id == id,
                Includes = "Airline"
            });
            if (flight == null) return NotFound();

            var selected = GetSessionFlights();
            ViewBag.IsSelected    = selected.Contains(id);
            ViewBag.SelectionCount = selected.Count;
            return View(flight);
        }

        // =========================
        // SELECT
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Select(int flightId)
        {
            var flight = _flights.Get(flightId);
            if (flight == null) return NotFound();

            var sessionFlights = GetSessionFlights();
            if (!sessionFlights.Contains(flightId)) sessionFlights.Add(flightId);

            SaveSessionFlights(sessionFlights);
            SaveCookieFlights(sessionFlights);

            TempData["Confirmation"] =
                $"Flight {flight.FlightCode} selected! You have {sessionFlights.Count} flight(s).";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // SELECTIONS
        // =========================

        public IActionResult Selections()
        {
            var ids     = GetSessionFlights();
            var flights = _flights.GetByIds(ids).ToList();
            ViewBag.SelectionCount = ids.Count;
            return View(flights);
        }

        // =========================
        // REMOVE / CLEAR
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFlight(int flightId)
        {
            var flights = GetSessionFlights();
            flights.Remove(flightId);
            SaveSessionFlights(flights);
            SaveCookieFlights(flights);

            var flight = _flights.Get(flightId);
            TempData["Confirmation"] = flight != null
                ? $"Flight {flight.FlightCode} removed from selections."
                : "Flight removed.";
            return RedirectToAction(nameof(Selections));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearAll()
        {
            HttpContext.Session.Remove(SESSION_KEY);
            Response.Cookies.Delete(COOKIE_KEY);
            TempData["Confirmation"] = "All selections cleared.";
            return RedirectToAction(nameof(Selections));
        }

        // =========================
        // BOOK
        // =========================

        public IActionResult Book()
        {
            var ids = GetSessionFlights();
            if (!ids.Any())
            {
                TempData["Error"] = "No flights selected. Please select flights first.";
                return RedirectToAction(nameof(Index));
            }

            var flights = _flights.GetByIds(ids).ToList();
            return View(new ReservationViewModel
            {
                SelectedFlights = flights,
                CabinTypes      = CabinTypes.GetAll(),
                TotalPrice      = flights.Sum(f => f.Price)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Book(ReservationViewModel model)
        {
            var ids     = GetSessionFlights();
            var flights = _flights.GetByIds(ids).ToList();

            model.SelectedFlights = flights;
            model.CabinTypes      = CabinTypes.GetAll();
            model.TotalPrice      = flights.Sum(f => f.Price) * model.PassengerCount;

            if (!flights.Any())
            {
                ModelState.AddModelError("", "No flights found. Please start over.");
                return View(model);
            }
            if (!ModelState.IsValid) return View(model);

            string confirmationNumber = "RES-" + Guid.NewGuid().ToString("N")[..8].ToUpper();

            try
            {
                foreach (var flight in flights)
                {
                    _reservations.Insert(new Reservation
                    {
                        ConfirmationNumber = confirmationNumber,
                        FlightId           = flight.Id,
                        PassengerName      = model.PassengerName.Trim(),
                        Email              = model.Email.Trim(),
                        CabinType          = model.CabinType,
                        PassengerCount     = model.PassengerCount,
                        TotalPrice         = flight.Price * model.PassengerCount,
                        BookingDate        = DateTime.Now
                    });
                }
                _reservations.Save();
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred saving your reservation. Please try again.");
                return View(model);
            }

            HttpContext.Session.Remove(SESSION_KEY);
            Response.Cookies.Delete(COOKIE_KEY);

            return RedirectToAction(nameof(Confirmation), new { confirmationNumber });
        }

        // =========================
        // CONFIRMATION
        // =========================

        public IActionResult Confirmation(string? confirmationNumber)
        {
            if (string.IsNullOrEmpty(confirmationNumber)) return RedirectToAction(nameof(Index));

            var reservations = _reservations.List(new QueryOptions<Reservation>
            {
                Where    = r => r.ConfirmationNumber == confirmationNumber,
                Includes = "Flight,Flight.Airline"
            }).ToList();

            if (!reservations.Any()) return NotFound();
            return View(reservations);
        }

        // =========================
        // VIEW MODEL BUILDER
        // =========================

        private SearchViewModel BuildViewModel(FlightFilter filter)
        {
            var options = new QueryOptions<Flight> { Includes = "Airline" };

            if (!string.IsNullOrEmpty(filter.From))
                options.Where = f => f.From == filter.From;
            if (!string.IsNullOrEmpty(filter.To))
            {
                var from = filter.From;
                if (!string.IsNullOrEmpty(from))
                    options.Where = f => f.From == from && f.To == filter.To;
                else
                    options.Where = f => f.To == filter.To;
            }

            // Re-read filter from session
            var savedFilterJson = HttpContext.Session.GetString("Filter");
            FlightFilter activeFilter = filter;
            if (!string.IsNullOrEmpty(savedFilterJson))
            {
                try { activeFilter = JsonSerializer.Deserialize<FlightFilter>(savedFilterJson) ?? filter; }
                catch { /* ignore */ }
            }

            var allFlights = _flights.GetAllWithAirline().ToList();

            // Apply filters
            IEnumerable<Flight> filtered = allFlights;
            if (!string.IsNullOrEmpty(activeFilter.From))
                filtered = filtered.Where(f => f.From == activeFilter.From);
            if (!string.IsNullOrEmpty(activeFilter.To))
                filtered = filtered.Where(f => f.To == activeFilter.To);
            if (activeFilter.DepartureDate.HasValue)
                filtered = filtered.Where(f => f.Date.Date == activeFilter.DepartureDate.Value.Date);
            if (!string.IsNullOrEmpty(activeFilter.CabinType) && activeFilter.CabinType != "All")
                filtered = filtered.Where(f => f.CabinType == activeFilter.CabinType);

            return new SearchViewModel
            {
                Flights        = filtered.ToList(),
                Airlines       = _airlines.List(new QueryOptions<Airline>()).ToList(),
                Filter         = activeFilter,
                SelectionCount = GetSessionFlights().Count,
                FromCities     = allFlights.Select(f => f.From).Where(c => c != null)
                                           .Distinct().OrderBy(c => c).Select(c => c!).ToList(),
                ToCities       = allFlights.Select(f => f.To).Where(c => c != null)
                                           .Distinct().OrderBy(c => c).Select(c => c!).ToList()
            };
        }
    }
}
