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

        // GET: Airlines/Flights/Index
        // Accepts optional filter params from the query string (GET form submission)
        public IActionResult Index(string? fromCity, string? toCity, string? cabinType)
        {
            var query = _context.Flights.Include(f => f.Airline).AsQueryable();

            if (!string.IsNullOrEmpty(fromCity))
                query = query.Where(f => f.From == fromCity);

            if (!string.IsNullOrEmpty(toCity))
                query = query.Where(f => f.To == toCity);

            if (!string.IsNullOrEmpty(cabinType))
                query = query.Where(f => f.CabinType == cabinType);

            var viewModel = new FlightViewModel
            {
                Flights       = query.OrderBy(f => f.Date).ToList(),
                Airlines      = _context.Airlines.ToList(),
                FromCities    = _context.Flights.Select(f => f.From).Where(f => f != null).Distinct().OrderBy(c => c).Select(f => f!).ToList(),
                ToCities      = _context.Flights.Select(f => f.To).Where(f => f != null).Distinct().OrderBy(c => c).Select(f => f!).ToList(),
                CabinTypes    = CabinTypes.GetAll(),
                AircraftTypes = AircraftTypes.GetAll(),
                SelectedFrom  = fromCity,
                SelectedTo    = toCity,
                SelectedCabin = cabinType
            };

            return View(viewModel);
        }

        // GET: Airlines/Flights/Manage
        public IActionResult Manage()
        {
            var viewModel = new FlightViewModel
            {
                Flights  = _context.Flights.Include(f => f.Airline).OrderByDescending(f => f.Date).ToList(),
                Airlines = _context.Airlines.ToList()
            };
            return View(viewModel);
        }

        // GET: Airlines/Flights/Create
        public IActionResult Create()
        {
            var viewModel = new DetailFlightViewModel
            {
                Flight        = new Flight(),
                Airlines      = _context.Airlines.ToList(),
                CabinTypes    = CabinTypes.GetAll(),
                AircraftTypes = AircraftTypes.GetAll()
            };
            return View(viewModel);
        }

        // POST: Airlines/Flights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            string?  FlightNumber,
            int      AirlineId,
            string?  From,
            string?  To,
            DateTime Date,
            string?  CabinType,
            string?  DepartureTime,
            string?  ArrivalTime,
            string?  AircraftType,
            decimal  Emission,
            decimal  Price)
        {
            if (string.IsNullOrEmpty(FlightNumber))
                ModelState.AddModelError("FlightNumber", "Flight Number is required.");
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

            if (!string.IsNullOrEmpty(FlightNumber))
            {
                bool isDuplicate = _context.Flights.Any(f =>
                    f.FlightNumber == FlightNumber && f.Date.Date == Date.Date);
                if (isDuplicate)
                    ModelState.AddModelError("FlightNumber", "This FlightCode + Date combination already exists.");
            }

            if (!ModelState.IsValid)
            {
                var vm = new DetailFlightViewModel
                {
                    Flight = new Flight { FlightNumber = FlightNumber, AirlineId = AirlineId, From = From, To = To, Date = Date, CabinType = CabinType, DepartureTime = DepartureTime, ArrivalTime = ArrivalTime, AircraftType = AircraftType, Emission = Emission, Price = Price },
                    Airlines = _context.Airlines.ToList(), CabinTypes = CabinTypes.GetAll(), AircraftTypes = AircraftTypes.GetAll()
                };
                return View(vm);
            }

            _context.Flights.Add(new Flight { FlightNumber = FlightNumber, AirlineId = AirlineId, From = From, To = To, Date = Date, CabinType = CabinType, DepartureTime = DepartureTime, ArrivalTime = ArrivalTime, AircraftType = AircraftType, Emission = Emission, Price = Price });
            _context.SaveChanges();
            TempData["Confirmation"] = $"Flight {FlightNumber} was created successfully.";
            return RedirectToAction(nameof(Manage));
        }

        // GET: Airlines/Flights/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var flight = _context.Flights.Find(id);
            if (flight == null) return NotFound();

            var viewModel = new DetailFlightViewModel
            {
                Flight        = flight,
                Airlines      = _context.Airlines.ToList(),
                CabinTypes    = CabinTypes.GetAll(),
                AircraftTypes = AircraftTypes.GetAll()
            };
            return View(viewModel);
        }

        // POST: Airlines/Flights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int      id,
            string?  FlightNumber,
            int      AirlineId,
            string?  From,
            string?  To,
            DateTime Date,
            string?  CabinType,
            string?  DepartureTime,
            string?  ArrivalTime,
            string?  AircraftType,
            decimal  Emission,
            decimal  Price)
        {
            if (string.IsNullOrEmpty(FlightNumber))
                ModelState.AddModelError("FlightNumber", "Flight Number is required.");
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

            if (!string.IsNullOrEmpty(FlightNumber))
            {
                bool isDuplicate = _context.Flights.Any(f =>
                    f.FlightNumber == FlightNumber && f.Date.Date == Date.Date && f.Id != id);
                if (isDuplicate)
                    ModelState.AddModelError("FlightNumber", "This FlightCode + Date combination already exists.");
            }

            if (!ModelState.IsValid)
            {
                var vm = new DetailFlightViewModel
                {
                    Flight = new Flight { Id = id, FlightNumber = FlightNumber, AirlineId = AirlineId, From = From, To = To, Date = Date, CabinType = CabinType, DepartureTime = DepartureTime, ArrivalTime = ArrivalTime, AircraftType = AircraftType, Emission = Emission, Price = Price },
                    Airlines = _context.Airlines.ToList(), CabinTypes = CabinTypes.GetAll(), AircraftTypes = AircraftTypes.GetAll()
                };
                return View(vm);
            }

            var flight = _context.Flights.Find(id);
            if (flight == null) return NotFound();

            flight.FlightNumber = FlightNumber; flight.AirlineId = AirlineId; flight.From = From;
            flight.To = To; flight.Date = Date; flight.CabinType = CabinType;
            flight.DepartureTime = DepartureTime; flight.ArrivalTime = ArrivalTime;
            flight.AircraftType = AircraftType; flight.Emission = Emission; flight.Price = Price;

            _context.Flights.Update(flight);
            _context.SaveChanges();
            TempData["Confirmation"] = $"Flight {flight.FlightNumber} was updated successfully.";
            return RedirectToAction(nameof(Manage));
        }

        // GET: Airlines/Flights/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var flight = _context.Flights.Include(f => f.Airline).FirstOrDefault(f => f.Id == id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        // POST: Airlines/Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var flight = _context.Flights.Find(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
                _context.SaveChanges();
                TempData["Confirmation"] = $"Flight {flight.FlightNumber} was deleted successfully.";
            }
            return RedirectToAction(nameof(Manage));
        }

        // GET: Airlines/Flights/Regulation
        public IActionResult Regulation() => View();

        // Remote validation
        [AcceptVerbs("GET", "POST")]
        public IActionResult IsFlightCodeDateUnique(string? flightNumber, DateTime? date, int id = 0)
        {
            if (string.IsNullOrEmpty(flightNumber) || date == null) return Json(true);
            bool isDuplicate = _context.Flights.Any(f =>
                f.FlightNumber == flightNumber && f.Date.Date == date.Value.Date && f.Id != id);
            return Json(!isDuplicate);
        }

        private bool FlightExists(int id) => _context.Flights.Any(e => e.Id == id);
    }
}
