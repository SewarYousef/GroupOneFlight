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
            public IActionResult Index(string? fromCity, string? toCity, string? cabinType)
            {
                var query = _context.Flights.Include(f => f.Airline).AsQueryable();
    
                if (!string.IsNullOrEmpty(fromCity))  query = query.Where(f => f.From == fromCity);
                if (!string.IsNullOrEmpty(toCity))    query = query.Where(f => f.To == toCity);
                if (!string.IsNullOrEmpty(cabinType)) query = query.Where(f => f.CabinType == cabinType);
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
            var viewModel = new FlightViewModel
            {
                Flights = _context.Flights
                                  .Include(f => f.Airline)
                                  .OrderByDescending(f => f.Date)
                                  .ToList(),
                Airlines = _context.Airlines.ToList()
            };

            return View(viewModel);
        }

        // Create Flight - GET
        public IActionResult Create()
        {
            var viewModel = new DetailFlightViewModel
            {
                Flight = new Flight(),
                Airlines = _context.Airlines.ToList(),
                CabinTypes = CabinTypes.GetAllCabinTypes(),
                AircraftTypes = AircraftTypes.GetAllAircraftTypes()
            };

            return View(viewModel);
        }

        // Create Flight - POST (PRG Pattern)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Flight flight)
        {
            bool alreadyCheckedByRemote = TempData["RemoteValidated"] as bool? == true;

            if (!alreadyCheckedByRemote && !string.IsNullOrEmpty(flight.FlightNumber))
            {
                bool isDuplicate = _context.Flights.Any(f =>
                    f.FlightNumber == flight.FlightNumber &&
                    f.Date.Date == flight.Date.Date &&
                    f.Id != flight.Id);

                if (isDuplicate)
                {
                    ModelState.AddModelError("FlightNumber",
                        "This FlightCode + Date combination already exists. " +
                        "Please use a different flight code or date.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Flights.Add(flight);
                _context.SaveChanges();
                
                // POST-Redirect-Get: Redirect to Manage after successful creation
                TempData["Confirmation"] = $"Flight {flight.FlightNumber} was created successfully.";
                
                return RedirectToAction(nameof(Manage));            }

            // If validation fails, re-render the form with error messages
            var viewModel = new DetailFlightViewModel
            {
                Flight = flight,
                Airlines = _context.Airlines.ToList(),
                CabinTypes = CabinTypes.GetAll(),
                AircraftTypes = AircraftTypes.GetAll()
            };

            return View(viewModel);
        }

        // Edit Flight - GET
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var flight = _context.Flights.Find(id);
            if (flight == null)
                return NotFound();

            var viewModel = new DetailFlightViewModel
            {
                Flight = flight,
                Airlines = _context.Airlines.ToList(),
                CabinTypes = CabinTypes.GetAllCabinTypes(),
                AircraftTypes = AircraftTypes.GetAllAircraftTypes()
            };

            return View(viewModel);
        }

        // Edit Flight - POST (PRG Pattern)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Flight flight)
        {
            if (id != flight.Id)
                return NotFound();

            bool alreadyCheckedByRemote = TempData["RemoteValidated"] as bool? == true;

            if (!alreadyCheckedByRemote && !string.IsNullOrEmpty(flight.FlightNumber))
            {
                bool isDuplicate = _context.Flights.Any(f =>
                    f.FlightNumber == flight.FlightNumber &&
                    f.Date.Date == flight.Date.Date &&
                    f.Id != flight.Id);   // exclude the flight being edited

                if (isDuplicate)
                {
                    ModelState.AddModelError("FlightNumber",
                        "This FlightCode + Date combination already exists. " +
                        "Please use a different flight code or date.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Flights.Update(flight);
                    _context.SaveChanges();

                    // POST-Redirect-Get: Redirect to Manage after successful edit
                    TempData["Confirmation"] = $"Flight {flight.FlightNumber} was updated successfully.";
                    return RedirectToAction(nameof(Manage));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.Id))
                        return NotFound();
                    throw;
                }
            }

            // If validation fails, re-render the form with error messages
            var viewModel = new DetailFlightViewModel
            {
                Flight = flight,
                Airlines = _context.Airlines.ToList(),
                CabinTypes = CabinTypes.GetAll(),
                AircraftTypes = AircraftTypes.GetAll()
            };

            return View(viewModel);
        }

        // Delete Flight - GET (Confirmation page)
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var flight = _context.Flights
                                 .Include(f => f.Airline)
                                 .FirstOrDefault(m => m.Id == id);

            if (flight == null)
                return NotFound();

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
            }

            // POST-Redirect-Get: Redirect to Manage after deletion
            return RedirectToAction("Manage", new { message = "Flight deleted successfully." });
        }

        // Regulation Information (Routing test)
        public IActionResult Regulation()
        {
            return View();
        }


        [AcceptVerbs("GET", "POST")]
        public IActionResult IsFlightCodeDateUnique(
            string? flightNumber,
            DateTime? date,
            int id = 0)
        {
            if (string.IsNullOrEmpty(flightNumber) || date == null)
                return Json(true); // Let other validators handle empty values

            bool isDuplicate = _context.Flights.Any(f =>
                f.FlightNumber == flightNumber &&
                f.Date.Date == date.Value.Date &&
                f.Id != id);

            if (isDuplicate)
            {
                // Do NOT set TempData here — this is the AJAX path.
                // TempData is only meaningful for the server-side POST fallback.
                return Json(false); // false → jQuery shows the [Remote] ErrorMessage
            }

            // Mark as validated so the POST action can skip the duplicate check
            TempData["RemoteValidated"] = true;
            return Json(true);
        }

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.Id == id);
        }
    }
}
