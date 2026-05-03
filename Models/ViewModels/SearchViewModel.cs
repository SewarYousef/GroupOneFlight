using System.Collections.Generic;
using GroupOneFlight.Models.DomainModels;

namespace GroupOneFlight.Models.ViewModels
{
    public class SearchViewModel
    {
        // Results
        public List<Flight> Flights { get; set; } = new();
        public List<Airline> Airlines { get; set; } = new();

        // Filtering state (what user selected)
        public FlightFilter Filter { get; set; } = new();

        // UI helpers
        public int SelectionCount { get; set; }

        // Dropdown data sources
        public List<string> FromCities { get; set; } = new();
        public List<string> ToCities { get; set; } = new();
        public List<string> CabinTypes { get; set; } = new();
        public List<string> AircraftTypes { get; set; } = new();
    }
}