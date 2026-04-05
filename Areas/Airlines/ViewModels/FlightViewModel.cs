using System.Collections.Generic;
using GroupOneFlight.Areas.Airlines.Models;

namespace GroupOneFlight.Areas.Airlines.ViewModels
{
    public class FlightViewModel
    {
        public List<Flight> Flights { get; set; } = new();
        public List<Airline> Airlines { get; set; } = new();
        public List<string> FromCities { get; set; } = new();
        public List<string> ToCities { get; set; } = new();
        public List<string> CabinTypes { get; set; } = new();
    }
}