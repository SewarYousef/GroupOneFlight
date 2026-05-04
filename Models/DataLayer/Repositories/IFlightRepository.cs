using System.Collections.Generic;
using GroupOneFlight.Models.DomainModels;

namespace GroupOneFlight.Models.DataLayer.Repositories
{
    public interface IFlightRepository : IRepository<Flight>
    {
        // Convenience methods specific to flights
        IEnumerable<Flight> GetAllWithAirline();
        IEnumerable<Flight> GetByIds(IEnumerable<int> ids);
        bool IsReserved(int flightId);
    }
}
