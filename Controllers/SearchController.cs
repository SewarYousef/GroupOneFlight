using GroupOneFlight.Models.DataLayer;
using GroupOneFlight.Models.ViewModels;
using GroupOneFlight.Models.DomainModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GroupOneFlight.Controllers
{
    public class SearchController : Controller
    {
        private readonly FlightDbContext _context;

        public SearchController(FlightDbContext context)
        {
            _context = context;
        }

        // =========================
        // SESSION HELPERS
        // =========================

        private const string SESSION_KEY = "SelectedFlights";

        private List<int> GetSessionFlights()
        {
            var data = HttpContext.Session.GetString(SESSION_KEY);
            return string.IsNullOrEmpty(data)
                ? new List<int>()
                : JsonSerializer.Deserialize<List<int>>(data) ?? new List<int>();
        }

        private void SaveSessionFlights(List<int> flights)
        {
            HttpContext.Session.SetString(SESSION_KEY,
                JsonSerializer.Serialize(flights));
        }

        // =========================
        // COOKIE HELPERS
        // =========================

        private const string COOKIE_KEY = "SelectedFlightsCookie";

        private List<int> GetCookieFlights()
        {
            var cookie = Request.Cookies[COOKIE_KEY];
            return string.IsNullOrEmpty(cookie)
                ? new List<int>()
                : JsonSerializer.Deserialize<List<int>>(cookie) ?? new List<int>();
        }

        private void SaveCookieFlights(List<int> flights)
        {
            Response.Cookies.Append(COOKIE_KEY,
                JsonSerializer.Serialize(flights),
                new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(7),
                    HttpOnly = true
                });
        }

        // =========================
        // INDEX
        // =========================

        public IActionResult Index()
        {
            var sessionFlights = GetSessionFlights();

            if (!sessionFlights.Any())
            {
                var cookieFlights = GetCookieFlights();
                if (cookieFlights.Any())
                {
                    SaveSessionFlights(cookieFlights);
                }
            }

            return View(BuildViewModel(new FlightFilter()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(FlightFilter filter)
        {
            HttpContext.Session.SetString("Filter",
                JsonSerializer.Serialize(filter));

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
            var flight = _context.Flights
                .Include(f => f.Airline)
                .FirstOrDefault(f => f.Id == id);

            if (flight == null) return NotFound();

            var selected = GetSessionFlights();

            ViewBag.IsSelected = selected.Contains(id);
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
            var flight = _context.Flights.FirstOrDefault(f => f.Id == flightId);
            if (flight == null) return NotFound();

            var sessionFlights = GetSessionFlights();

            if (!sessionFlights.Contains(flightId))
                sessionFlights.Add(flightId);

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
            var ids = GetSessionFlights();

            var flights = _context.Flights
                .Include(f => f.Airline)
                .Where(f => ids.Contains(f.Id))
                .ToList();

            ViewBag.SelectionCount = ids.Count;

            return View(flights);
        }

        // =========================
        // REMOVE
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFlight(int flightId)
        {
            var flights = GetSessionFlights();

            flights.Remove(flightId);

            SaveSessionFlights(flights);
            SaveCookieFlights(flights);

            var flight = _context.Flights.FirstOrDefault(f => f.Id == flightId);

            TempData["Confirmation"] =
                flight != null
                ? $"Flight {flight.FlightCode} removed from selections."
                : "Flight removed.";

            return RedirectToAction(nameof(Selections));
        }

        // =========================
        // CLEAR ALL
        // =========================

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

            var flights = _context.Flights
                .Include(f => f.Airline)
                .Where(f => ids.Contains(f.Id))
                .ToList();

            var vm = new ReservationViewModel
            {
                SelectedFlights = flights,
                CabinTypes = CabinTypes.GetAll(),
                TotalPrice = flights.Sum(f => f.Price)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Book(ReservationViewModel model)
        {
            var ids = GetSessionFlights();
            var flights = _context.Flights
                .Include(f => f.Airline)
                .Where(f => ids.Contains(f.Id))
                .ToList();

            // Repopulate non-bound properties before any possible return
            model.SelectedFlights = flights;
            model.CabinTypes = CabinTypes.GetAll();
            model.TotalPrice = flights.Sum(f => f.Price) * model.PassengerCount;

            if (!flights.Any())
            {
                ModelState.AddModelError("", "No flights found for your selection. Please start over.");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            // Generate a short, readable confirmation number
            string confirmationNumber = "RES-" + Guid.NewGuid().ToString("N")[..8].ToUpper();

            try
            {
                foreach (var flight in flights)
                {
                    _context.Reservations.Add(new Reservation
                    {
                        ConfirmationNumber = confirmationNumber,
                        FlightId = flight.Id,
                        PassengerName = model.PassengerName.Trim(),
                        Email = model.Email.Trim(),
                        CabinType = model.CabinType,
                        PassengerCount = model.PassengerCount,
                        TotalPrice = flight.Price * model.PassengerCount,
                        BookingDate = DateTime.Now
                    });
                }

                _context.SaveChanges();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while saving your reservation. Please try again.");
                return View(model);
            }

            // Clear session & cookie selections after successful booking
            HttpContext.Session.Remove(SESSION_KEY);
            Response.Cookies.Delete(COOKIE_KEY);

            return RedirectToAction(nameof(Confirmation), new { confirmationNumber });
        }

        // =========================
        // CONFIRMATION
        // =========================

        public IActionResult Confirmation(string? confirmationNumber)
        {
            if (string.IsNullOrEmpty(confirmationNumber))
                return RedirectToAction(nameof(Index));

            var reservations = _context.Reservations
                .Include(r => r.Flight)
                .ThenInclude(f => f!.Airline)
                .Where(r => r.ConfirmationNumber == confirmationNumber)
                .ToList();

            if (!reservations.Any()) return NotFound();

            return View(reservations);
        }

        // =========================
        // VIEW MODEL BUILDER
        // =========================

        private SearchViewModel BuildViewModel(FlightFilter filter)
        {
            var query = _context.Flights
                .Include(f => f.Airline)
                .AsQueryable();

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
                Flights = query.ToList(),
                Airlines = _context.Airlines.ToList(),
                Filter = filter,
                SelectionCount = GetSessionFlights().Count,

                FromCities = _context.Flights
                    .Select(f => f.From)
                    .Where(c => c != null)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList()!,

                ToCities = _context.Flights
                    .Select(f => f.To)
                    .Where(c => c != null)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList()!
            };
        }
    }
}
