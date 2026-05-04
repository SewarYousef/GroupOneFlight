using Microsoft.AspNetCore.Mvc;
using GroupOneFlight.Models.ViewModels;
using GroupOneFlight.Models.DataLayer;
using GroupOneFlight.Models.DataLayer.Repositories;
using GroupOneFlight.Models.DomainModels;

namespace GroupOneFlight.Areas.Airlines.Controllers
{
    [Area("Airlines")]
    public class FlightsController : Controller
    {
        private readonly IFlightRepository    _flights;
        private readonly IRepository<Airline> _airlines;
        private readonly IRepository<Reservation> _reservations;

        public FlightsController(
            IFlightRepository        flights,
            IRepository<Airline>     airlines,
            IRepository<Reservation> reservations)
        {
            _flights      = flights;
            _airlines     = airlines;
            _reservations = reservations;
        }

        // Lists 
        
        // GET: Airlines/Flights/Index
        public IActionResult Index(string? fromCity, string? toCity, string? cabinType)
        {
            var options = new QueryOptions<Flight> { Includes = "Airline" };

            if (!string.IsNullOrEmpty(fromCity) && !string.IsNullOrEmpty(toCity) && !string.IsNullOrEmpty(cabinType))
                options.Where = f => f.From == fromCity && f.To == toCity && f.CabinType == cabinType;
            else if (!string.IsNullOrEmpty(fromCity) && !string.IsNullOrEmpty(toCity))
                options.Where = f => f.From == fromCity && f.To == toCity;
            else if (!string.IsNullOrEmpty(fromCity))
                options.Where = f => f.From == fromCity;
            else if (!string.IsNullOrEmpty(toCity))
                options.Where = f => f.To == toCity;
            else if (!string.IsNullOrEmpty(cabinType))
                options.Where = f => f.CabinType == cabinType;

            options.OrderBy = f => f.Date;

            var allFlights = _flights.GetAllWithAirline().ToList();

            var viewModel = new FlightIndexViewModel
            {
                Flights = _flights.List(options).ToList(),
                FromCities = allFlights.Select(f => f.From).Where(f => f != null)
                                       .Distinct().OrderBy(c => c).Select(f => f!).ToList(),
                ToCities   = allFlights.Select(f => f.To).Where(f => f != null)
                                       .Distinct().OrderBy(c => c).Select(f => f!).ToList(),
                CabinTypes   = CabinTypes.GetAll(),
                SelectedFrom  = fromCity,
                SelectedTo    = toCity,
                SelectedCabin = cabinType
            };

            return View(viewModel);
        }

        // GET: Airlines/Flights/Manage
        public IActionResult Manage()
        {
            var allFlights = _flights.GetAllWithAirline().ToList();

            var viewModel = new FlightIndexViewModel
            {
                Flights    = allFlights,
                FromCities = allFlights.Select(f => f.From).Where(f => f != null)
                                       .Distinct().OrderBy(c => c).Select(f => f!).ToList(),
                ToCities   = allFlights.Select(f => f.To).Where(f => f != null)
                                       .Distinct().OrderBy(c => c).Select(f => f!).ToList(),
                CabinTypes = CabinTypes.GetAll()
            };

            return View(viewModel);
        }

        // Create 
        
        // GET: Airlines/Flights/Create
        public IActionResult Create()
        {
            return View(new FlightFormViewModel
            {
                Flight       = new Flight(),
                Airlines     = _airlines.List(new QueryOptions<Airline> { OrderBy = a => a.Name }).ToList(),
                CabinTypes   = CabinTypes.GetAll(),
                AircraftTypes = AircraftTypes.GetAll()
            });
        }

        // POST: Airlines/Flights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            string? FlightCode, int AirlineId, string? From, string? To,
            DateTime Date, string? CabinType, string? DepartureTime,
            string? ArrivalTime, string? AircraftType, decimal Emission, decimal Price)
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
                bool isDuplicate = _flights.List(new QueryOptions<Flight>
                {
                    Where = f => f.FlightCode == FlightCode && f.Date.Date == Date.Date
                }).Any();
                if (isDuplicate)
                    ModelState.AddModelError("FlightCode", "This FlightCode + Date combination already exists.");
            }

            if (!ModelState.IsValid)
                return View(RebuildFormViewModel(FlightCode, AirlineId, From, To, Date,
                    CabinType, DepartureTime, ArrivalTime, AircraftType, Emission, Price));

            try
            {
                _flights.Insert(new Flight
                {
                    FlightCode = FlightCode, AirlineId = AirlineId, From = From, To = To,
                    Date = Date, CabinType = CabinType, DepartureTime = DepartureTime,
                    ArrivalTime = ArrivalTime, AircraftType = AircraftType,
                    Emission = Emission, Price = Price
                });
                _flights.Save();
                TempData["Confirmation"] = $"Flight {FlightCode} was created successfully.";
                return RedirectToAction(nameof(Manage));
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                return View(RebuildFormViewModel(FlightCode, AirlineId, From, To, Date,
                    CabinType, DepartureTime, ArrivalTime, AircraftType, Emission, Price));
            }
        }

        // Edit 
        
        // GET: Airlines/Flights/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var flight = _flights.Get(id.Value);
            if (flight == null) return NotFound();

            return View(new FlightFormViewModel
            {
                Flight        = flight,
                Airlines      = _airlines.List(new QueryOptions<Airline> { OrderBy = a => a.Name }).ToList(),
                CabinTypes    = CabinTypes.GetAll(),
                AircraftTypes = AircraftTypes.GetAll()
            });
        }

        // POST: Airlines/Flights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int id, string? FlightCode, int AirlineId, string? From, string? To,
            DateTime Date, string? CabinType, string? DepartureTime,
            string? ArrivalTime, string? AircraftType, decimal Emission, decimal Price)
        {
            var flight = _flights.Get(id);
            if (flight == null) return NotFound();

            flight.FlightCode    = FlightCode;
            flight.AirlineId     = AirlineId;
            flight.From          = From;
            flight.To            = To;
            flight.Date          = Date;
            flight.CabinType     = CabinType;
            flight.DepartureTime = DepartureTime;
            flight.ArrivalTime   = ArrivalTime;
            flight.AircraftType  = AircraftType;
            flight.Emission      = Emission;
            flight.Price         = Price;

            try
            {
                _flights.Update(flight);
                _flights.Save();
                TempData["Confirmation"] = $"Flight {flight.FlightCode} was updated successfully.";
                return RedirectToAction(nameof(Manage));
            }
            catch
            {
                TempData["Error"] = "An error occurred while updating. Please try again.";
                return RedirectToAction(nameof(Edit), new { id });
            }
        }

        // Delete 
        
        // GET: Airlines/Flights/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var flight = _flights.Get(new QueryOptions<Flight>
            {
                Where    = f => f.Id == id,
                Includes = "Airline"
            });
            if (flight == null) return NotFound();

            ViewBag.HasReservations  = _flights.IsReserved(id.Value);
            ViewBag.ReservationCount = _reservations.List(
                new QueryOptions<Reservation> { Where = r => r.FlightId == id }).Count();

            return View(new DetailFlightViewModel { Flight = flight });
        }

        // POST: Airlines/Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var flight = _flights.Get(id);
            if (flight == null)
            {
                TempData["Error"] = "Flight not found.";
                return RedirectToAction(nameof(Manage));
            }

            if (_flights.IsReserved(id))
            {
                TempData["Error"] =
                    $"Flight {flight.FlightCode} cannot be deleted — it has active reservations.";
                return RedirectToAction(nameof(Manage));
            }

            try
            {
                _flights.Delete(flight);
                _flights.Save();
                TempData["Confirmation"] = $"Flight {flight.FlightCode} deleted successfully.";
            }
            catch
            {
                TempData["Error"] = $"An error occurred while deleting flight {flight.FlightCode}.";
            }

            return RedirectToAction(nameof(Manage));
        }

        // Regulation / Remote validation
        
        public IActionResult Regulation() => View(new FlightRegulationViewModel());

        [AcceptVerbs("GET", "POST")]
        public IActionResult IsFlightCodeDateUnique(string? flightCode, DateTime? date, int id = 0)
        {
            if (string.IsNullOrEmpty(flightCode) || date == null) return Json(true);

            bool isDuplicate = _flights.List(new QueryOptions<Flight>
            {
                Where = f => f.FlightCode == flightCode &&
                             f.Date.Date == date.Value.Date &&
                             f.Id != id
            }).Any();

            return Json(!isDuplicate);
        }

        // Helpers 
        
        private FlightFormViewModel RebuildFormViewModel(
            string? FlightCode, int AirlineId, string? From, string? To,
            DateTime Date, string? CabinType, string? DepartureTime,
            string? ArrivalTime, string? AircraftType, decimal Emission, decimal Price) =>
            new FlightFormViewModel
            {
                Flight = new Flight
                {
                    FlightCode = FlightCode, AirlineId = AirlineId,
                    From = From, To = To, Date = Date, CabinType = CabinType,
                    DepartureTime = DepartureTime, ArrivalTime = ArrivalTime,
                    AircraftType = AircraftType, Emission = Emission, Price = Price
                },
                Airlines      = _airlines.List(new QueryOptions<Airline> { OrderBy = a => a.Name }).ToList(),
                CabinTypes    = CabinTypes.GetAll(),
                AircraftTypes = AircraftTypes.GetAll()
            };
    }
}
