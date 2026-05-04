using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using GroupOneFlight.Models.DomainModels;

namespace GroupOneFlight.Models.DataLayer.Repositories
{
    public class FlightRepository : Repository<Flight>, IFlightRepository
    {
        public FlightRepository(FlightDbContext ctx) : base(ctx) { }

        /// <summary>Returns all flights with their Airline navigation property loaded.</summary>
        public IEnumerable<Flight> GetAllWithAirline() =>
            context.Flights
                   .Include(f => f.Airline)
                   .OrderByDescending(f => f.Date)
                   .ToList();

        /// <summary>Returns flights whose Ids are in the provided list (used for selections).</summary>
        public IEnumerable<Flight> GetByIds(IEnumerable<int> ids) =>
            context.Flights
                   .Include(f => f.Airline)
                   .Where(f => ids.Contains(f.Id))
                   .ToList();

        /// <summary>Returns true if the flight has at least one reservation.</summary>
        public bool IsReserved(int flightId) =>
            context.Reservations.Any(r => r.FlightId == flightId);
    }
}
