using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupOneFlight.Models.ViewModels;
using GroupOneFlight.Models.DataLayer;
using GroupOneFlight.Models.DomainModels;

namespace GroupOneFlight.Areas.Airlines.Controllers
{
    [Area("Airlines")]
    public class FlightsController : Controller
    {
        private readonly FlightDbContext _context;

        public FlightsController(FlightDbContext context)
        {
            _context = context;
        }

        // Temporary lookup data (replaces missing helper classes)
        private static readonly List<string> CabinTypesList = new()
        {
            "Economy",
            "Premium Economy",
            "Business",
            "First"
        };

        private static readonly List<string> AircraftTypesList = new()
        {
            "Boeing 737",
            "Boeing 777",
            "Airbus A320",
            "Airbus A350"
        };

        // GET: Airlines/Flights/Index
       public IActionResult Index(string? fromCity, string? toCity, string? cabinType)
{
    var query = _context.Flights
        .Include(f => f.Airline)
        .AsQueryable();

    if (!string.IsNullOrEmpty(fromCity))
        query = query.Where(f => f.From == fromCity);

    if (!string.IsNullOrEmpty(toCity))
        query = query.Where(f => f.To == toCity);

    if (!string.IsNullOrEmpty(cabinType))
        query = query.Where(f => f.CabinType == cabinType);

    var viewModel = new FlightIndexViewModel
    {
        Flights = query.OrderBy(f => f.Date).ToList(),

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

        SelectedFrom = fromCity,
        SelectedTo = toCity,
        SelectedCabin = cabinType
    };

    return View(viewModel);
}

        // GET: Airlines/Flights/Manage
        public IActionResult Manage()
        {
            var viewModel = new FlightIndexViewModel
            {
                Flights = _context.Flights
                    .Include(f => f.Airline)
                    .OrderByDescending(f => f.Date)
                    .ToList(),

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

                CabinTypes = CabinTypes.GetAll()
            };

            return View(viewModel);
        }

        // GET: Airlines/Flights/Create
        public IActionResult Create()
        {
            var viewModel = new FlightFormViewModel
            {
                Flight = new Flight(),
                Airlines = _context.Airlines.ToList(),
                CabinTypes = CabinTypesList,
                AircraftTypes = AircraftTypesList
            };

            return View(viewModel);
        }

        // POST: Airlines/Flights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            string? FlightCode,
            int AirlineId,
            string? From,
            string? To,
            DateTime Date,
            string? CabinType,
            string? DepartureTime,
            string? ArrivalTime,
            string? AircraftType,
            decimal Emission,
            decimal Price)
        {
            if (string.IsNullOrEmpty(FlightCode))
                ModelState.AddModelError("FlightCode", "Flight Code is required.");

            if (string.IsNullOrEmpty(From))
                ModelState.AddModelError("From", "From city is required.");

            if (string.IsNullOrEmpty(To))
                ModelState.AddModelError("To", "To city is required.");

            if (AirlineId <= 0)
                ModelState.AddModelError("AirlineId", "Airline is required.");

            if (string.IsNullOrEmpty(CabinType))
                ModelState.AddModelError("CabinType", "Cabin Type is required.");

            if (Date <= DateTime.Today)
                ModelState.AddModelError("Date", "Date must be after today.");

            if (!string.IsNullOrEmpty(FlightCode))
            {
                bool isDuplicate = _context.Flights.Any(f =>
                    f.FlightCode == FlightCode && f.Date.Date == Date.Date);

                if (isDuplicate)
                    ModelState.AddModelError("FlightCode", "This FlightCode + Date combination already exists.");
            }

            if (!ModelState.IsValid)
            {
                var vm = new FlightFormViewModel
                {
                    Flight = new Flight
                    {
                        FlightCode = FlightCode,
                        AirlineId = AirlineId,
                        From = From,
                        To = To,
                        Date = Date,
                        CabinType = CabinType,
                        DepartureTime = DepartureTime,
                        ArrivalTime = ArrivalTime,
                        AircraftType = AircraftType,
                        Emission = Emission,
                        Price = Price
                    },
                    Airlines = _context.Airlines.ToList(),
                    CabinTypes = CabinTypesList,
                    AircraftTypes = AircraftTypesList
                };

                return View(vm);
            }

            try
            {
                _context.Flights.Add(new Flight
                {
                    FlightCode = FlightCode,
                    AirlineId = AirlineId,
                    From = From,
                    To = To,
                    Date = Date,
                    CabinType = CabinType,
                    DepartureTime = DepartureTime,
                    ArrivalTime = ArrivalTime,
                    AircraftType = AircraftType,
                    Emission = Emission,
                    Price = Price
                });

                _context.SaveChanges();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while saving the flight. Please try again.");

                var vm = new FlightFormViewModel
                {
                    Flight = new Flight
                    {
                        FlightCode = FlightCode,
                        AirlineId = AirlineId,
                        From = From,
                        To = To,
                        Date = Date,
                        CabinType = CabinType,
                        DepartureTime = DepartureTime,
                        ArrivalTime = ArrivalTime,
                        AircraftType = AircraftType,
                        Emission = Emission,
                        Price = Price
                    },
                    Airlines = _context.Airlines.ToList(),
                    CabinTypes = CabinTypesList,
                    AircraftTypes = AircraftTypesList
                };

                return View(vm);
            }

            TempData["Confirmation"] = $"Flight {FlightCode} was created successfully.";
            return RedirectToAction(nameof(Manage));
        }

        // GET: Airlines/Flights/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var flight = _context.Flights.Find(id);
            if (flight == null) return NotFound();

            var viewModel = new FlightFormViewModel
            {
                Flight = flight,
                Airlines = _context.Airlines.ToList(),
                CabinTypes = CabinTypesList,
                AircraftTypes = AircraftTypesList
            };

            return View(viewModel);
        }

        // POST: Airlines/Flights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int id,
            string? FlightCode,
            int AirlineId,
            string? From,
            string? To,
            DateTime Date,
            string? CabinType,
            string? DepartureTime,
            string? ArrivalTime,
            string? AircraftType,
            decimal Emission,
            decimal Price)
        {
            var flight = _context.Flights.Find(id);
            if (flight == null) return NotFound();

            flight.FlightCode = FlightCode;
            flight.AirlineId = AirlineId;
            flight.From = From;
            flight.To = To;
            flight.Date = Date;
            flight.CabinType = CabinType;
            flight.DepartureTime = DepartureTime;
            flight.ArrivalTime = ArrivalTime;
            flight.AircraftType = AircraftType;
            flight.Emission = Emission;
            flight.Price = Price;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while updating the flight. Please try again.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            TempData["Confirmation"] = $"Flight {flight.FlightCode} was updated successfully.";
            return RedirectToAction(nameof(Manage));
        }

        // GET: Airlines/Flights/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var flight = _context.Flights
                .Include(f => f.Airline)
                .FirstOrDefault(f => f.Id == id);

            if (flight == null) return NotFound();

            // Check for existing reservations and surface it in the view
            ViewBag.HasReservations = _context.Reservations.Any(r => r.FlightId == id);
            ViewBag.ReservationCount = _context.Reservations.Count(r => r.FlightId == id);

            var viewModel = new DetailFlightViewModel
            {
                Flight = flight
            };

            return View(viewModel);
        }

        // POST: Airlines/Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var flight = _context.Flights.Find(id);

            if (flight == null)
            {
                TempData["Error"] = "Flight not found.";
                return RedirectToAction(nameof(Manage));
            }

            // Block deletion if the flight has active reservations
            bool hasReservations = _context.Reservations.Any(r => r.FlightId == id);
            if (hasReservations)
            {
                TempData["Error"] =
                    $"Flight {flight.FlightCode} cannot be deleted because it has active reservations.";
                return RedirectToAction(nameof(Manage));
            }

            try
            {
                _context.Flights.Remove(flight);
                _context.SaveChanges();
                TempData["Confirmation"] =
                    $"Flight {flight.FlightCode} was deleted successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] =
                    $"An error occurred while deleting flight {flight.FlightCode}. Please try again.";
            }

            return RedirectToAction(nameof(Manage));
        }

        // GET: Airlines/Flights/Regulation
        public IActionResult Regulation()
        {
            var model = new FlightRegulationViewModel();
            return View(model);
        }

        // Remote validation
        [AcceptVerbs("GET", "POST")]
        public IActionResult IsFlightCodeDateUnique(string? flightCode, DateTime? date, int id = 0)
        {
            if (string.IsNullOrEmpty(flightCode) || date == null)
                return Json(true);

            bool isDuplicate = _context.Flights.Any(f =>
                f.FlightCode == flightCode &&
                f.Date.Date == date.Value.Date &&
                f.Id != id);

            return Json(!isDuplicate);
        }

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.Id == id);
        }
    }
}
