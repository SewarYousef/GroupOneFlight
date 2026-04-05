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
            if (string.IsNullOrEmpty(flight.FlightNumber))
                ModelState.AddModelError("FlightNumber", "Flight Number is required.");
            if (string.IsNullOrEmpty(flight.From))
                ModelState.AddModelError("From", "From city is required.");
            if (string.IsNullOrEmpty(flight.To))
                ModelState.AddModelError("To", "To city is required.");
            if (flight.AirlineId <= 0)
                ModelState.AddModelError("AirlineId", "Airline is required.");

            if (ModelState.IsValid)
            {
                _context.Flights.Add(flight);
                _context.SaveChanges();
                
                // POST-Redirect-Get: Redirect to Manage after successful creation
                return RedirectToAction("Manage", new { message = "Flight created successfully." });
            }

            // If validation fails, re-render the form with error messages
            var viewModel = new DetailFlightViewModel
            {
                Flight = flight,
                Airlines = _context.Airlines.ToList(),
                CabinTypes = CabinTypes.GetAllCabinTypes(),
                AircraftTypes = AircraftTypes.GetAllAircraftTypes()
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

            if (string.IsNullOrEmpty(flight.FlightNumber))
                ModelState.AddModelError("FlightNumber", "Flight Number is required.");
            if (string.IsNullOrEmpty(flight.From))
                ModelState.AddModelError("From", "From city is required.");
            if (string.IsNullOrEmpty(flight.To))
                ModelState.AddModelError("To", "To city is required.");
            if (flight.AirlineId <= 0)
                ModelState.AddModelError("AirlineId", "Airline is required.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Flights.Update(flight);
                    _context.SaveChanges();

                    // POST-Redirect-Get: Redirect to Manage after successful edit
                    return RedirectToAction("Manage", new { message = "Flight updated successfully." });
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
                CabinTypes = CabinTypes.GetAllCabinTypes(),
                AircraftTypes = AircraftTypes.GetAllAircraftTypes()
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
            return Content("Airline Regulation Page");
        }

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.Id == id);
        }
    }
}