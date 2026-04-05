using System.Collections.Generic;
using GroupOneFlight.Areas.Airlines.Models;

namespace GroupOneFlight.Areas.Airlines.ViewModels
{
    public class DetailFlightViewModel
    {
        // Current flight and its options
        public Flight? Flight { get; set; }
        public FlightOptions? FlightOptions { get; set; }
        public Airline? Airline { get; set; }

        // Filtering criteria retained from search
        public string? SelectedFrom { get; set; }
        public string? SelectedTo { get; set; }
        public string? SelectedCabin { get; set; }

        // Lists for dropdowns/displays
        public List<Airline> Airlines { get; set; } = new();
        public List<string> CabinTypes { get; set; } = new();
        public List<string> AircraftTypes { get; set; } = new();

        // Pricing display
        public Dictionary<string, decimal> PriceByType { get; set; } = new();
        public Dictionary<string, int> AvailableSeatsByType { get; set; } = new();
    }
}
