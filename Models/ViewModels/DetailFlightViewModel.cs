using System.Collections.Generic;
using GroupOneFlight.Models.DomainModels;

namespace GroupOneFlight.Models.ViewModels
{
    public class DetailFlightViewModel
    {
        public Flight? Flight { get; set; }

        // Related data
        public Airline? Airline { get; set; }
        public FlightOptions? FlightOptions { get; set; }

        // Dropdowns
        public List<Airline> Airlines { get; set; } = new();
        public List<string> CabinTypes { get; set; } = new();
        public List<string> AircraftTypes { get; set; } = new();

        // Optional UI state
        public string? SelectedFrom { get; set; }
        public string? SelectedTo { get; set; }
        public string? SelectedCabin { get; set; }

        // Optional pricing display
        public Dictionary<string, decimal> PriceByType { get; set; } = new();
        public Dictionary<string, int> AvailableSeatsByType { get; set; } = new();
    }
}