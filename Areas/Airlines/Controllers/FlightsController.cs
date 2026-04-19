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
                                     .ToList(),
                AircraftTypes = AircraftTypes.GetAllAircraftTypes(),
                Airlines = _context.Airlines.ToList()
            };

            return View(viewModel);
        }

        // Manage Flights (List view)
        public IActionResult Manage()
        {
            var flights = _context.Flights
                                  .Include(f => f.Airline)
                                  .OrderByDescending(f => f.Date)
                                  .ToList();

            var viewModel = new FlightViewModel
            {
                Flights = flights,
                Airlines = _context.Airlines.ToList()
            };

            return View(viewModel);
        }

        // Create Flight - GET
        public IActionResult Create()
        {
            var viewModel = new DetailFlightViewModel
            {
                Flight        = new Flight(),
                Airlines      = _context.Airlines.ToList(),
                CabinTypes    = CabinTypes.GetAllCabinTypes(),
                AircraftTypes = AircraftTypes.GetAllAircraftTypes()
            };
            return View(viewModel);
        }

        // Create Flight - POST (PRG Pattern)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Flight flight)
        {
            if (string.IsNullOrEmpty(flight.FlightNumber))
                ModelState.AddModelError("FlightNumber", "Flight Number is required.");
            if (string.IsNullOrEmpty(flight.From))
                ModelState.AddModelError("From", "From city is required.");
            if (string.IsNullOrEmpty(flight.To))
                ModelState.AddModelError("To", "To city is required.");
            if (flight.AirlineId <= 0)
                ModelState.AddModelError("AirlineId", "Airline is required.");
            // Server-side fallback: runs uniqueness check if JS was disabled
            bool alreadyChecked = TempData["RemoteValidated"] as bool? == true;
            if (!alreadyChecked && !string.IsNullOrEmpty(flight.FlightNumber))
            {
                bool isDuplicate = _context.Flights.Any(f =>
                    f.FlightNumber == flight.FlightNumber &&
                    f.Date.Date == flight.Date.Date &&
                    f.Id != flight.Id);

                if (isDuplicate)
                    ModelState.AddModelError("FlightNumber",
                        "This FlightCode + Date combination already exists. " +
                        "Please use a different flight code or date.");
            }

            if (ModelState.IsValid)
            {
                _context.Flights.Add(flight);
                _context.SaveChanges();
                TempData["Confirmation"] = $"Flight {flight.FlightNumber} was created successfully.";
                return RedirectToAction(nameof(Manage));
            }

            // Validation failed — re-render form with error messages
            var viewModel = new DetailFlightViewModel
            {
                Flight        = flight,
                Airlines      = _context.Airlines.ToList(),
                CabinTypes    = CabinTypes.GetAllCabinTypes(),
                AircraftTypes = AircraftTypes.GetAllAircraftTypes()
            };
            return View(viewModel);
        }

        // Edit Flight - GET
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var flight = _context.Flights.Find(id);
            if (flight == null) return NotFound();

            var viewModel = new DetailFlightViewModel
            {
                Flight        = flight,
                Airlines      = _context.Airlines.ToList(),
                CabinTypes    = CabinTypes.GetAllCabinTypes(),
                AircraftTypes = AircraftTypes.GetAllAircraftTypes()
            };
            return View(viewModel);
        }

        // Edit Flight - POST (PRG Pattern)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Flight flight)
        {
            if (id != flight.Id) return NotFound();

            if (string.IsNullOrEmpty(flight.FlightNumber))
                ModelState.AddModelError("FlightNumber", "Flight Number is required.");
            if (string.IsNullOrEmpty(flight.From))
                ModelState.AddModelError("From", "From city is required.");
            if (string.IsNullOrEmpty(flight.To))
                ModelState.AddModelError("To", "To city is required.");
            if (flight.AirlineId <= 0)
                ModelState.AddModelError("AirlineId", "Airline is required.");
            // Server-side fallback: runs uniqueness check if JS was disabled
            bool alreadyCheckedEdit = TempData["RemoteValidated"] as bool? == true;
            if (!alreadyCheckedEdit && !string.IsNullOrEmpty(flight.FlightNumber))
            {
                bool isDuplicate = _context.Flights.Any(f =>
                    f.FlightNumber == flight.FlightNumber &&
                    f.Date.Date == flight.Date.Date &&
                    f.Id != flight.Id);

                if (isDuplicate)
                    ModelState.AddModelError("FlightNumber",
                        "This FlightCode + Date combination already exists. " +
                        "Please use a different flight code or date.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Flights.Update(flight);
                    _context.SaveChanges();
                    TempData["Confirmation"] = $"Flight {flight.FlightNumber} was updated successfully.";
                    return RedirectToAction(nameof(Manage));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.Id)) return NotFound();
                    throw;
                }
            }

            // Validation failed — re-render form with error messages
            var viewModel = new DetailFlightViewModel
            {
                Flight        = flight,
                Airlines      = _context.Airlines.ToList(),
                CabinTypes    = CabinTypes.GetAllCabinTypes(),
                AircraftTypes = AircraftTypes.GetAllAircraftTypes()
            };
            return View(viewModel);
        }

        // Delete Flight - GET (Confirmation page)
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var flight = _context.Flights
                                 .Include(f => f.Airline)
                                 .FirstOrDefault(m => m.Id == id);

            if (flight == null) return NotFound();
            return View(flight);
        }

        // Delete Flight - POST (PRG Pattern)
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

        // Regulation page
        public IActionResult Regulation()
        {
            return View();
        }
        // Remote validation: checks FlightNumber + Date uniqueness via AJAX
        [AcceptVerbs("GET", "POST")]
        public IActionResult IsFlightCodeDateUnique(
            string? flightNumber,
            DateTime? date,
            int id = 0)
        {
            if (string.IsNullOrEmpty(flightNumber) || date == null)
                return Json(true);

            bool isDuplicate = _context.Flights.Any(f =>
                f.FlightNumber == flightNumber &&
                f.Date.Date == date.Value.Date &&
                f.Id != id);

            if (isDuplicate)
                return Json(false);

            TempData["RemoteValidated"] = true;
            return Json(true);
        }

        private bool FlightExists(int id) =>
            _context.Flights.Any(e => e.Id == id);
    }
}
