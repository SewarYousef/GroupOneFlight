using System.Collections.Generic;
using GroupOneFlight.Models.DomainModels;

namespace GroupOneFlight.Models.ViewModels
{
    public class FlightFormViewModel
    {
        public Flight Flight { get; set; } = new();

        public List<Airline> Airlines { get; set; } = new();

        public List<string> CabinTypes { get; set; } = new();
        public List<string> AircraftTypes { get; set; } = new();
    }
}